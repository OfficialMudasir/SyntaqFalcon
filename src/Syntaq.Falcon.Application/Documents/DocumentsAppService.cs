using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Organizations;
using Abp.UI;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.API;
using Syntaq.Falcon.Apps;
using Syntaq.Falcon.Apps.Dtos;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Documents.Models;
using Syntaq.Falcon.EntityFrameworkCore.Repositories;
using Syntaq.Falcon.Files;
using Syntaq.Falcon.Files.Dtos;
using Syntaq.Falcon.Folders;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Submissions;
using Syntaq.Falcon.Submissions.Dtos;
using Syntaq.Falcon.Utility;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.Projects;
using Microsoft.AspNetCore.Http;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Forms.Dtos;
using Hangfire.Storage;

namespace Syntaq.Falcon.Documents
{
    [EnableCors("AllowAll")]
    [AbpAuthorize(AppPermissions.Pages_Documents)]
    public class DocumentsAppService : FalconAppServiceBase, IDocumentsAppService
    {
        private readonly SubmissionManager _submissionManager;
        private readonly ACLManager _ACLManager;
        private readonly FilesManager _filesManager;
        private readonly FolderManager _folderManager;
        private readonly DocumentAssemblyMessageProvider _documentAssemblyMessageProvider;
        private readonly RecordManager _recordManager;
        private readonly DocumentAssemblyManager _documentAssemblyManager;
        private readonly APIManager _APIManager;

        private readonly IRepository<Syntaq.Falcon.Apps.App, Guid> _appRepository;
        private readonly IRepository<Syntaq.Falcon.Forms.Form, Guid> _formRepository;

        private readonly IRepository<AppJob, Guid> _appJobRepository;
        private readonly IRepository<RecordMatter, Guid> _recordMatterRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly ICustomMergeTextRepository _mergeTextRepository;
        private readonly IRepository<Record, Guid> _recordRepository;
        private readonly IRepository<Template, Guid> _templateRepository;

        private readonly IRepository<ACL, int> _aclRepository;

        private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IRepository<RecordMatterItem, Guid> _recordMatterItemRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DocumentsAppService(
            SubmissionManager submissionManager,
            IRepository<RecordMatter, Guid> recordMatterRepository,
            ACLManager aclManager,
            FilesManager filesManager,
            FolderManager folderManager,
            DocumentAssemblyMessageProvider documentAssemblyMessageProvider,
            RecordManager recordManager,
            DocumentAssemblyManager documentAssemblyManager,
            APIManager aPIManager,
            IRepository<Syntaq.Falcon.Apps.App, Guid> appAppRepository,
            IRepository<AppJob, Guid> appJobRepository,
            IRepository<Form, Guid> formJobRepository,
            IRepository<User, long> userRepository,
            IRepository<ACL, int> aclRepository,
            IRepository<Record, Guid> recordRepository,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            ICustomMergeTextRepository mergeTextRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Project, Guid> projectRepository,
            IRepository<RecordMatterItem, Guid> recordMatterItemRepository,
             IHttpContextAccessor httpContextAccessor,
            IRepository<Template, Guid> templateRepository)
        {
            _submissionManager = submissionManager;
            _recordMatterRepository = recordMatterRepository;
            _ACLManager = aclManager;
            _filesManager = filesManager;
            _folderManager = folderManager;
            _documentAssemblyMessageProvider = documentAssemblyMessageProvider;
            _mergeTextRepository = mergeTextRepository;
            _recordManager = recordManager;
            _documentAssemblyManager = documentAssemblyManager;
            _APIManager = aPIManager;
            _appJobRepository = appJobRepository;
            _formRepository = formJobRepository;
            _appRepository = appAppRepository;
            _userRepository = userRepository;
            _organizationUnitRepository = organizationUnitRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _aclRepository = aclRepository;
            _mergeTextRepository = mergeTextRepository;
            _recordRepository = recordRepository;
            _templateRepository = templateRepository;
            _projectRepository = projectRepository;
            _recordMatterItemRepository = recordMatterItemRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Step 1: "Building the Message" - Document Assembly Preprocessor, massages the data as required by the system
        /// </summary>
        /// <param name="JSONObject">JSON Object containing all form data and workflow data needed for document automation</param>
        /// 
        [AbpAllowAnonymous]
        public async Task Automate(dynamic JSONObject)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings { DateParseHandling = DateParseHandling.None };
            List<CreateOrEditAppJobDto> AssemblyJobs = new List<CreateOrEditAppJobDto>();
            List<AppJob> jobs = JSONObject.Job;

            var userId = AbpSession.UserId;
            var tenantId = AbpSession.TenantId;

            FormDto form = null;
            Guid? formid = null;
            Guid? appid = null;

            JObject SubData = JsonConvert.DeserializeObject<JObject>(JSONObject.data);
            // Todo Server checking            
            var ispaid = Convert.ToBoolean(GetJArrayValue(SubData, "IsPaid"));
            bool LockOnBuild = false;
            if (DynamicUtility.IsPropertyExist(JSONObject, "FormId"))
            {
                formid = new Guid(JSONObject.FormId);
                form = _formRepository.GetAll().Select(f =>
                    new FormDto() { Id = f.Id, PaymentEnabled = f.PaymentEnabled, LockOnBuild = f.LockOnBuild, OriginalId = f.OriginalId })
                    .FirstOrDefault(e => e.Id == formid);

                if (form != null)
                {
                    // If not paid and requires payment If not paid then do not lock
                    // Document will have draft watermark on it
                    if (ispaid && form.PaymentEnabled)
                    {
                        LockOnBuild = form.LockOnBuild;
                    }
                }
            }

            Guid SubmissionId = GetJArrayValue(SubData, "SubmissionId") == null ? JSONObject.SubmissionId == null ? Guid.NewGuid() : new Guid(Convert.ToString(JSONObject.SubmissionId)) : new Guid(GetJArrayValue(SubData, "SubmissionId"));
            _submissionManager.UpdateSubmissionStatus(new CreateOrEditSubmissionDto()
            {
                Id = SubmissionId,
                FormId = DynamicUtility.IsPropertyExist(JSONObject, "FormId") ? new Guid(Convert.ToString(JSONObject.FormId)) : (Guid?)null,
                AppId = DynamicUtility.IsPropertyExist(JSONObject, "appId") ? new Guid(Convert.ToString(JSONObject.appId)) : (Guid?)null,
                SubmissionStatus = "Submitted",
                // VoucherAmount = DynamicUtility.IsPropertyExist(JSONObject, "VoucherAmount") ? Convert.ToDecimal(JSONObject.VoucherAmount) : 0
            });

            Guid? entityid = null;
            if (DynamicUtility.IsPropertyExist(JSONObject, "FormId"))
                entityid = new Guid(Convert.ToString(JSONObject.FormId));

            if (DynamicUtility.IsPropertyExist(JSONObject, "appId"))
                entityid = new Guid(Convert.ToString(JSONObject.appId));

            if (!ValidateSubmission(SubData, entityid))
            {
                _submissionManager.UpdateSubmissionStatus(new CreateOrEditSubmissionDto()
                {
                    Id = SubmissionId,
                    FormId = DynamicUtility.IsPropertyExist(JSONObject, "FormId") ? new Guid(Convert.ToString(JSONObject.FormId)) : (Guid?)null,
                    AppId = DynamicUtility.IsPropertyExist(JSONObject, "appId") ? new Guid(Convert.ToString(JSONObject.appId)) : (Guid?)null,
                    SubmissionStatus = "Rejected",
                    Description = "Submission Rejected. Rules Violation.",
                    VoucherAmount = DynamicUtility.IsPropertyExist(JSONObject, "VoucherAmount") ? Convert.ToDecimal(JSONObject.VoucherAmount) : 0
                });
                return;
            }

            // If a form is run / submitted anonymously then take the user context of the Form owner
            // only applies to forms not Apps which require a logged on user
            if (userId == null)
            {
                // If there is no user session default to the App or Form owner
                if (DynamicUtility.IsPropertyExist(JSONObject, "FormId"))
                {
                    // Get the Owner and tenant of the Form
                    formid = new Guid(JSONObject.FormId);

                    form = _formRepository.GetAll().Select(f =>
                        new FormDto() { Id = f.Id, PaymentEnabled = f.PaymentEnabled, LockOnBuild = f.LockOnBuild, OriginalId = f.OriginalId })
                        .FirstOrDefault(i => i.Id == formid || i.OriginalId == formid);

                    if (form != null)
                    {
                        var acl = _aclRepository.GetAll()
                            .Select(a => new { a.EntityID, a.Role, a.UserId })
                            .FirstOrDefault(i => (i.EntityID == formid || i.EntityID == form.OriginalId) && i.Role == "O");
                        userId = acl.UserId;
                    }
                }

                if (DynamicUtility.IsPropertyExist(JSONObject, "appId"))
                {
                    var appId = new Guid(Convert.ToString(JSONObject.appId));

                    var acl = _aclRepository.GetAll()
                        .Select(a => new { a.EntityID, a.Role, a.UserId })
                        .FirstOrDefault(i => i.EntityID == appId && i.Role == "O");

                    userId = acl.UserId;
                }

                var user = UserManager.Users
                    .Select(u => new { u.Id, u.TenantId })
                    .FirstOrDefault(i => i.Id == userId);
                if (user != null)
                    tenantId = user.TenantId;
            }

            if (jobs.Count() <= 0)
            {
                jobs.Add(new AppJob()
                {
                    Name = "Default",
                    Data = "{\"Name\": \"Default\",\"Data\": null,\"AppId\": \"00000000-0000-0000-0000-000000000000\",\"EntityId\": \"00000000-0000-0000-0000-000000000000\",\"TenantId\": " + (tenantId != null ? tenantId.ToString() : "null") + ",\"Form\": null,\"Document\": null,\"RecordMatter\": null,\"User\": {\"ID\": " + userId + ",\"Name\": null,\"Email\": \"\",\"Permission\": \"E\"},\"WorkFlow\": null,\"XData\": null,\"Id\": \"00000000-0000-0000-0000-000000000000\"}",
                    AppId = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    EntityId = Guid.Parse("00000000-0000-0000-0000-000000000000")
                });
            }

            jobs.ForEach(h =>
            {
                //CreateOrEditAppJobDto jobClass = JsonConvert.DeserializeObject<CreateOrEditAppJobDto>(h.Data, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
                CreateOrEditAppJobDto jobClass = JsonConvert.DeserializeObject<CreateOrEditAppJobDto>(h.Data);

                jobClass.Data = JSONObject.data;

                if (DynamicUtility.IsPropertyExist(JSONObject, "summaryTableHTML"))
                    jobClass.SummaryTableHTML = JSONObject.summaryTableHTML;

                JObject ufData = JsonConvert.DeserializeObject<JObject>(JSONObject.data);

                ExpandoObject desiredFormat = new ExpandoObject();
                foreach (KeyValuePair<string, JToken> key in ufData)
                {
                    string MainKey = key.Key;
                    if (!string.IsNullOrEmpty(MainKey))
                    {
                        SubDataFormat(MainKey, MainKey, ufData[MainKey], ref desiredFormat);
                    }
                }

                RecordSet recordSet = null;

                bool HasACLAccess = true;
                string AccessError = null;

                // Get RecordId if supplied or generate new one
                // Get New RecordMatterId 
                Guid recordId = GetJArrayValue(ufData, "RecordId") == null ? Guid.NewGuid() : new Guid(GetJArrayValue(ufData, "RecordId"));
                Guid recordMatterId = Guid.NewGuid();
                Guid recordMatterItemId = Guid.NewGuid();

                // If recordmatterId was supplied lookup recordmatter and set RecordID
                if (GetJArrayValue(ufData, "RecordMatterId") != null)
                {
                    recordMatterId = new Guid(GetJArrayValue(ufData, "RecordMatterId"));
                    var recordMatter = _recordMatterRepository.GetAll().Where(i => i.Id == recordMatterId)
                        .Select(rm => new RecordMatterDto
                        {
                            RecordId = rm.RecordId,
                        })
                        .FirstOrDefault();
                }
                recordMatterItemId = GetJArrayValue(ufData, "RecordMatterItemId") == null ? Guid.NewGuid() : new Guid(GetJArrayValue(ufData, "RecordMatterItemId"));

                if (jobClass.RecordMatter == null)
                {
                    jobClass.RecordMatter = new List<CreateOrEditAppJobRecordMatterDto>{
                        new CreateOrEditAppJobRecordMatterDto()
                        {
                            FolderName = "Your Records",
                            RecordId = recordId,
                            RecordMatterId = recordMatterId,
                            RecordMatterItemId = recordMatterItemId
                        }
                    };
                }
                else
                {
                    jobClass.RecordMatter.ForEach(i => {
                        i.RecordId = i.RecordId == Guid.Parse("00000000-0000-0000-0000-000000000000") ? recordId : i.RecordId;
                        i.RecordMatterId = i.RecordMatterId == Guid.Parse("00000000-0000-0000-0000-000000000000") ? recordMatterId : i.RecordMatterId;
                        i.RecordMatterItemId = i.RecordMatterItemId == Guid.Parse("00000000-0000-0000-0000-000000000000") ? recordMatterItemId : i.RecordMatterItemId;
                    });
                }

                var accessToken = DynamicUtility.IsPropertyExist(JSONObject, "AnonAuthToken") ? JSONObject.AnonAuthToken : null;

                if (!_ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = recordId, UserId = AbpSession.UserId, AccessToken = accessToken, TenantId = AbpSession.TenantId }).IsAuthed)
                {
                    throw new UserFriendlyException("Not Authorised");
                }

                if (!_ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = recordMatterId, UserId = AbpSession.UserId, AccessToken = accessToken, TenantId = AbpSession.TenantId }).IsAuthed)
                {
                    throw new UserFriendlyException("Not Authorised");
                }

                var sACLs = Convert.ToString(ufData["assignAcl"]);
                JArray jsonACLs = null;
                if (!string.IsNullOrEmpty(sACLs))
                {
                    jsonACLs = JArray.Parse(Convert.ToString(ufData["assignAcl"]));
                    if (jsonACLs != null)
                    {
                        jobClass.RecordMatter[0].Users = jobClass.RecordMatter[0].Users == null ? new List<CreateOrEditAppJobUserDto>() : jobClass.RecordMatter[0].Users;
                        using (var unitOfWork = _unitOfWorkManager.Begin())
                        {
                            foreach (var jsonACL in jsonACLs)
                            {

                                var aclpermission = jsonACL["Permission"] != null ? jsonACL["Permission"].ToString() : "V";
                                var aclname = jsonACL["Name"]?.ToString() ?? "";
                                var acltype = jsonACL["Type"]?.ToString() ?? "User";

                                if (acltype.ToLower() == "team")
                                {
                                    var orgunit = _organizationUnitRepository
                                        .GetAllList()
                                        .Select(o => new { o.Id, o.DisplayName, o.TenantId })
                                        .FirstOrDefault(i => i.DisplayName.ToLower() == aclname.ToLower() && i.TenantId == tenantId);
                                    if (orgunit != null)
                                    {
                                        if (!jobClass.RecordMatter[0].Teams.Any(i => i.ID == orgunit.Id))
                                        {
                                            CreateOrEditAppJobTeamDto TeamDto = new CreateOrEditAppJobTeamDto()
                                            {
                                                ID = orgunit.Id,
                                                Name = orgunit.DisplayName,
                                                Permission = aclpermission
                                            };
                                            jobClass.RecordMatter[0].Teams.Add(TeamDto);
                                        }
                                    }
                                }

                                if (acltype.ToLower() == "user")
                                {
                                    var user = _userRepository
                                        .GetAllList()
                                        .Select(u => new { u.Id, u.TenantId, u.UserName, u.EmailAddress })
                                        .FirstOrDefault(i => i.UserName.ToLower() == aclname.ToLower() && i.TenantId == tenantId);
                                    if (user != null)
                                    {
                                        if (!jobClass.RecordMatter[0].Users.Any(i => i.ID == user.Id))
                                        {
                                            CreateOrEditAppJobUserDto UserDto = new CreateOrEditAppJobUserDto()
                                            {
                                                ID = user.Id,
                                                Name = user.UserName,
                                                Email = user.EmailAddress,
                                                Permission = aclpermission
                                            };
                                            jobClass.RecordMatter[0].Users.Add(UserDto);
                                        }
                                    }
                                }

                            }
                            unitOfWork.Complete();
                        }
                    }
                }


                if (DynamicUtility.IsPropertyExist(JSONObject, "appId"))
                    appid = JSONObject.appId;

                jobClass.FormId = jobClass.FormId ?? formid;
                jobClass.AppId = jobClass.AppId ?? appid;

                jobClass.RecordMatter.ForEach(async j =>
                {
                    //TODO: Pipeline Restructure
                    if (userId == 0)
                    {
                        HasACLAccess = false;
                        AccessError += "Assembly RecordMatter must have UserID - Assembly Halted,";
                    }
                    else
                    {
                        JObject fileData = JsonConvert.DeserializeObject<JObject>(jobClass.Data);

                        // contributorId 
                        if (DynamicUtility.IsPropertyExist(fileData, "ContributorId"))
                            jobClass.ContributorId = (Guid)fileData["ContributorId"];

                        if (DynamicUtility.IsPropertyExist(fileData, "ContributorStatus"))
                            jobClass.ContributorStatus = (string)fileData["ContributorStatus"];

                        bool hasFiles = false;
                        foreach (KeyValuePair<string, JToken> key in fileData)
                        {
                            string MainKey = key.Key;
                            if (!string.IsNullOrEmpty(MainKey) && fileData[MainKey].Type == JTokenType.Array && fileData[MainKey].HasValues)
                            {
                                // Try catch used because cannot test to set if value is null first.
                                // causes a null execption and stops processing
                                try
                                {
                                    if (fileData[MainKey][0].Value<bool>("file") == true) { hasFiles = true; }
                                    else if (fileData[MainKey][0].Value<bool>("repeat") == true)
                                    {
                                        hasFiles = FindUploadFileinRepeat(hasFiles, fileData[MainKey]);

                                    }
                                }
                                catch { }
                            }
                        }

                        var recordname = GetFieldValue(Convert.ToString(JsonConvert.SerializeObject(desiredFormat)), j.RecordName);
                        var recordmattername = GetFieldValue(Convert.ToString(JsonConvert.SerializeObject(desiredFormat)), j.RecordMatterName);
                        recordSet = new RecordSet()
                        {
                            UserId = userId, //?? long.Parse(jobClass.User.ID.ToString(), System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowLeadingSign),
                            ACLPermission = "O",
                            //OrganizationId = j.OrganizationId != null ? j.OrganizationId : null,
                            TenantId = tenantId,
                            SubmissionData = jobClass.Data,
                            RecordId = j.RecordId != null ? Guid.Parse(j.RecordId.ToString()) : Guid.Parse("00000000-0000-0000-0000-000000000000"),
                            RecordName = recordname.ToString(),
                            RecordMatterId = j.RecordMatterId != null ? Guid.Parse(j.RecordMatterId.ToString()) : Guid.Parse("00000000-0000-0000-0000-000000000000"),
                            RecordMatterName = recordmattername.ToString(),
                            HasFiles = hasFiles,
                            RecordMatterItemId = j.RecordMatterItemId != null ? Guid.Parse(j.RecordMatterItemId.ToString()) : Guid.Parse("00000000-0000-0000-0000-000000000000"),
                            RecordMatterItemGroupId = j.RecordMatterItemId != null ? Guid.Parse(j.RecordMatterItemId.ToString()) : Guid.Parse("00000000-0000-0000-0000-000000000000"), //groupId = recordmatterId, parsing from form schema, project, recordmatterId=recordmatterId, butfor form, rmiId is different from form schema rmiId
                            //SubmissionId = j.SubmissionId != null ? Guid.Parse(j.SubmissionId.ToString()) : Guid.Parse("00000000-0000-0000-0000-000000000000"),
                            //SubmissionId = GetJArrayValue(ufData, "SubmissionId") != "00000000-0000-0000-0000-000000000000" ? Guid.Parse(GetJArrayValue(ufData, "SubmissionId")) : Guid.Parse("00000000-0000-0000-0000-000000000000"),
                            SubmissionId = SubmissionId,
                            FormId = formid,
                            AppId = appid,
                            FolderId = j.FolderId != null ? Guid.Parse(j.FolderId.ToString()) : Guid.Parse("00000000-0000-0000-0000-000000000000"),
                            ParentFolderId = Guid.Parse("00000000-0000-0000-0000-000000000000"), //Always the root folder???
                            FolderName = !string.IsNullOrEmpty(j.FolderName) ? j.FolderName : "Your Records",
                            FolderType = "R",
                            AnonAuthToken = DynamicUtility.IsPropertyExist(JSONObject, "AnonAuthToken") ? JSONObject.AnonAuthToken : null,
                            DocumentName = jobClass.Document?.Count() >= 1 ? jobClass.Document.First().DocumentName : string.Empty,
                            LockOnBuild = LockOnBuild
                        };
                        (recordSet, HasACLAccess, AccessError) = await _recordManager.SaveRecordSet(recordSet, false);

                        SubmissionId = recordSet.SubmissionId;
                        _submissionManager.UpdateSubmission(new CreateOrEditSubmissionDto()
                        {
                            //Id = GetJArrayValue(SubData, "SubmissionId") == null ? Guid.NewGuid() : new Guid(GetJArrayValue(SubData, "SubmissionId")),
                            Id = recordSet.SubmissionId,
                            UserId = userId,
                            RecordId = recordSet.RecordId,
                            RecordMatterId = recordSet.RecordMatterId
                        });


                        // If this is a Project do not save the recordmatteritem. These are only update on the submit process
                        if (_projectRepository.GetAll().Any(p => p.RecordId == recordSet.RecordId))
                        {
                            if (hasFiles)
                            {
                                var rmi = _recordMatterItemRepository
                                    .GetAll()
                                    .Select(rmi => new { rmi.RecordMatterId, rmi.GroupId })
                                    .FirstOrDefault(m => m.RecordMatterId == recordSet.RecordMatterId);

                                if (rmi != null) //if project has saved a rmi to recordmatter, keep the rmi groupId same
                                {
                                    recordSet.RecordMatterItemGroupId = rmi.GroupId;
                                }
                            }
                        }

                        foreach (KeyValuePair<string, JToken> key in fileData)
                        {
                            string MainKey = key.Key;
                            if (!string.IsNullOrEmpty(MainKey) && fileData[MainKey].Type == JTokenType.Array && fileData[MainKey].HasValues)
                            {
                                // Try catch used because cannot test to set if value is null first.
                                // causes a null execption and stops processing
                                try
                                {
                                    if (fileData[MainKey][0].Value<bool>("file") == true)
                                    {
                                        foreach (JToken file in (JArray)fileData[MainKey])
                                        {
                                            if (file.HasValues)
                                            {
                                                FileDto FileDto = JsonConvert.DeserializeObject<FileDto>(file.ToString());
                                                _filesManager.SaveFile(new SaveFileDto()
                                                {
                                                    RecordId = recordSet.RecordId,
                                                    RecordMatterId = recordSet.RecordMatterId,
                                                    RecordMatterItemGroupId = recordSet.RecordMatterItemGroupId,
                                                    File = FileDto,
                                                    AccessToken = DynamicUtility.IsPropertyExist(JSONObject, "AnonAuthToken") ? JSONObject.AnonAuthToken : null
                                                });
                                            }
                                        }
                                    }
                                    else if (fileData[MainKey][0].Value<bool>("repeat") == true)
                                    {
                                        string AccessToken = DynamicUtility.IsPropertyExist(JSONObject, "AnonAuthToken") ? JSONObject.AnonAuthToken : null;
                                        SaveUploadFileInRepeatPanel(fileData[MainKey], recordSet, AccessToken);

                                    }
                                }
                                catch { }
                            }
                        }
                    }

                    if (!HasACLAccess)
                    {
                        //throw new UserFriendlyException(AccessError);

                    }

                    if (jsonACLs != null)
                    {

                        j.Users = j.Users == null ? new List<CreateOrEditAppJobUserDto>() : j.Users;
                        using (var unitOfWork = _unitOfWorkManager.Begin())
                        {
                            foreach (var jsonACL in jsonACLs)
                            {
                                var aclpermission = jsonACL["Permission"] != null ? jsonACL["Permission"].ToString() : "V";
                                var aclname = jsonACL["Name"]?.ToString() ?? "";
                                var acltype = jsonACL["Type"]?.ToString() ?? "User";

                                if (acltype.ToLower() == "team")
                                {

                                    var orgunit = _organizationUnitRepository
                                        .GetAllList()
                                        .Select(rm => new
                                        {
                                            rm.Id,
                                            rm.DisplayName,
                                            rm.TenantId
                                        })
                                        .FirstOrDefault(i => i.DisplayName.ToLower() == aclname.ToLower() && i.TenantId == tenantId);

                                    if (orgunit != null)
                                    {
                                        if (!j.Teams.Any(i => i.ID == orgunit.Id))
                                        {
                                            CreateOrEditAppJobTeamDto TeamDto = new CreateOrEditAppJobTeamDto()
                                            {
                                                ID = orgunit.Id,
                                                Name = orgunit.DisplayName,
                                                Permission = aclpermission
                                            };
                                            j.Teams.Add(TeamDto);
                                        }
                                    }
                                }

                                if (acltype.ToLower() == "user")
                                {
                                    var user = _userRepository
                                        .GetAllList()
                                        .Select(rm => new
                                        {
                                            rm.Id,
                                            rm.UserName,
                                            rm.EmailAddress,
                                            rm.TenantId
                                        })
                                        .FirstOrDefault(i => i.UserName.ToLower() == aclname.ToLower() && i.TenantId == tenantId);

                                    if (user != null)
                                    {
                                        if (!j.Users.Any(i => i.ID == user.Id))
                                        {
                                            CreateOrEditAppJobUserDto UserDto = new CreateOrEditAppJobUserDto()
                                            {
                                                ID = user.Id,
                                                Name = user.UserName,
                                                Email = user.EmailAddress,
                                                Permission = aclpermission,
                                            };
                                            j.Users.Add(UserDto);
                                        }
                                    }
                                }
                            }
                            unitOfWork.Complete();
                        }
                    }

                    j.Users?.ForEach(async k =>
                    {
                        if (recordSet.ParentFolderId.ToString() != "00000000-0000-0000-0000-000000000000")
                        {

                            ACL aCL = new ACL() { EntityID = recordSet.RecordId, UserId = k.ID, Role = k.Permission, TenantId = tenantId, Type = "Record" };
                            await _ACLManager.AddACL(aCL);
                        }
                        else
                        {
                            //ACL aCL = new ACL() { EntityID = recordSet.FolderId, OrganizationUnitId = k.ID, Role = k.Permission };
                            ACL aCL = new ACL() { EntityID = recordSet.RecordId, UserId = k.ID, Role = k.Permission, TenantId = tenantId, Type = "Record" };
                            await _ACLManager.AddACL(aCL);
                        }
                    });

                    j.Teams?.ForEach(async k =>
                    {
                        if (recordSet.ParentFolderId.ToString() != "00000000-0000-0000-0000-000000000000")
                        {
                            //ACL aCL = new ACL() { EntityID = recordSet.FolderId, OrganizationUnitId = k.ID, Role = k.Permission };
                            ACL aCL = new ACL() { EntityID = recordSet.RecordId, OrganizationUnitId = k.ID, Role = k.Permission, TenantId = tenantId, Type = "Record" };
                            await _ACLManager.AddACL(aCL);
                        }
                        else
                        {
                            //ACL aCL = new ACL() { EntityID = recordSet.FolderId, OrganizationUnitId = k.ID, Role = k.Permission };
                            ACL aCL = new ACL() { EntityID = recordSet.RecordId, OrganizationUnitId = k.ID, Role = k.Permission, TenantId = tenantId, Type = "Record" };
                            await _ACLManager.AddACL(aCL);
                        }
                    });

                    j.TenantId = tenantId;
                    j.FolderId = recordSet.FolderId;
                    j.RecordId = recordSet.RecordId;
                    j.RecordMatterId = recordSet.RecordMatterId;
                    j.RecordMatterItemId = recordSet.RecordMatterItemId;
                    j.RecordMatterItemGroupId = recordSet.RecordMatterItemId;
                    j.SubmissionId = recordSet.SubmissionId;
                    j.OrganizationId = recordSet.OrganizationId;
                });

                List<XElement> XData = new List<XElement>();

                if (jobClass.Document == null || jobClass.Document.Count == 0)
                {
                    string Data = JsonConvert.SerializeObject(desiredFormat).ToString();
                    XElement xdata = XElement.Parse(JsonConvert.DeserializeXNode(Data, "data").ToString());

                    // Add New line \r \r\n field values based on if MText Exists

                    XData.Add(xdata);
                }
                else
                {
                    //int docId = 1;
                    foreach (CreateOrEditAppJobDocumentDto item in jobClass.Document)
                    {

                        //var documenttemplate = _templateRepository.FirstOrDefault(e => e.Id == item.DocumentId);
                        // Need to get the tea

                        dynamic iDocument = new ExpandoObject();
                        iDocument.Document = new ExpandoObject();
                        iDocument.Document.TenantId = AbpSession.TenantId; // Use to determine if document can be loaded by the assembly function


                        iDocument.Document.DocumentId = item.DocumentId;
                        iDocument.Document.DocumentTemplateURL = item.DocumentTemplateURL;
                        iDocument.Document.DocumentName = item.DocumentName;

                        string Data = JsonConvert.SerializeObject(desiredFormat).ToString();
                        XElement xdata = XElement.Parse(JsonConvert.DeserializeXNode(Data, "data").ToString());

                        dynamic system = JObject.FromObject(iDocument);
                        XElement xsystem = XElement.Parse(JsonConvert.DeserializeXNode(system.ToString(), "System").ToString());

                        // SET Mtext
                        var mtext = GetMtext(jobClass);
                        XElement xmtext = XElement.Parse(mtext);
                        xsystem.Element("Document").Add(xmtext);

                        XmlNode node = null;
                        if (!string.IsNullOrEmpty(item.FilterRule))
                        {
                            try
                            {
                                XmlDocument doc = new XmlDocument();
                                doc.LoadXml(JsonConvert.DeserializeXNode(jobClass.Data.ToString(), "Data").ToString());
                                node = doc.DocumentElement.SelectSingleNode(item.FilterRule);
                            }
                            catch (Exception) { }
                        }
                        else
                        {
                            node = new XmlDocument();
                        }

                        if (node != null || string.IsNullOrEmpty(item.FilterRule))
                        {
                            _documentAssemblyMessageProvider.RepeatCountSet(xdata);
                            _documentAssemblyMessageProvider.RepeatIDsSet(xdata, 1, 1);
                            xdata.Add(xsystem);
                        }

                        XData.Add(xdata);

                        SetBLNL(xdata);
                    }
                }

                jobClass.XData = XData;

                //Workflow
                if (jobClass.WorkFlow != null)
                {
                    jobClass.WorkFlow.BeforeAssembly?.ForEach(n =>
                    {
                        try
                        {
                            //TODO: handle output differently if Workflow is Async or not
                            n.URL += "&uid=" + Guid.NewGuid(); // Attempt at avoiding caching

                            Task<dynamic> output = _APIManager.TriggerAPI(n, jobClass, new List<PostProcessingItem>());

                            string outputresult = string.Empty;

                            if (Convert.ToBoolean(n.Async))
                            {
                                outputresult = output.Result.Result;
                            }
                            else
                            {
                                outputresult = output.Result;
                            }

                            if (!string.IsNullOrEmpty(outputresult))
                            {
                                dynamic result = output.Result;
                                dynamic jsonout = result.Result;

                                JObject jOut = JObject.Parse(jsonout);
                                JToken jOutRoot = jOut.Root;

                                // 
                                if (jOutRoot["data"] != null)
                                {
                                    string sDataOut = jOutRoot["data"].ToString();

                                    XElement xdata = XElement.Parse(JsonConvert.DeserializeXNode("{\"data\": " + sDataOut + "}").ToString());
                                    XElement system = jobClass.XData[0].Element("System");
                                    xdata.Add(system);

                                    jobClass.XData = new List<XElement>
                                {
                                    xdata
                                };
                                }

                                if (jOutRoot["Job"] != null)
                                {
                                    string sJobOut = jOutRoot["Job"].ToString();

                                    CreateOrEditAppJobDto jobOut = JsonConvert.DeserializeObject<CreateOrEditAppJobDto>(sJobOut);

                                    // Update the Job out with new values
                                    if (jobOut != null)
                                    {
                                        jobClass.Form = jobOut.Form;
                                        jobClass.RecordMatter = jobOut.RecordMatter;
                                        jobClass.User = jobClass.User;
                                        jobClass.WorkFlow.AfterAssembly = jobOut.WorkFlow.AfterAssembly;
                                        jobClass.WorkFlow.Email = jobOut.WorkFlow.Email;
                                        jobClass.Document = jobOut.Document;
                                    }
                                }

                                // Inject not Root File and non Job Json into job structure
                                foreach (JToken jitem in jOutRoot.Children())
                                {
                                    JProperty jProperty = jitem.ToObject<JProperty>();
                                    string propertyName = jProperty.Name;

                                    if (propertyName.ToLower() != "data" && propertyName.ToLower() != "job")
                                    {
                                        string sitem = "{ root: {" + jitem.ToString() + " }}";
                                        XElement LData = XElement.Parse(JsonConvert.DeserializeXNode(sitem).ToString());

                                        if (jobClass.XData.Count > 0)
                                        {
                                            XElement xdata = jobClass.XData[0];
                                            xdata.Elements(propertyName).Remove();

                                            foreach (XElement xitem in xdata.Elements())
                                            {
                                                XElement copy = XElement.Parse(xitem.ToString());
                                                LData.Add(copy);
                                            }
                                        }
                                    }
                                }
                            }

                            if (jobClass.XData.Count > 0)
                            {
                                _documentAssemblyMessageProvider.RepeatCountSet(jobClass.XData[0]);
                                _documentAssemblyMessageProvider.RepeatIDsSet(jobClass.XData[0], 1, 1);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new UserFriendlyException("Error Running Before Assembly API Definition: check data and run app again" + Environment.NewLine + ex.Message);
                        }
                    });
                }

                AssemblyJobs.Add(jobClass);
            });

            //var ispaid = Convert.ToBoolean(GetJArrayValue(SubData, "IsPaid"));
            _submissionManager.UpdateSubmission(new CreateOrEditSubmissionDto()
            {
                Id = SubmissionId,
                RequiresPayment = form == null ? false : form.PaymentEnabled,
                PaymentStatus = ispaid ? "Paid" : form == null ? "Not Required" : form.PaymentEnabled == true ? "Required" : "Not Required",
                UserId = userId,
                TenantId = tenantId,
                FormId = formid,
                AppId = appid,
                RecordId = AssemblyJobs.First().RecordMatter.First().RecordId,
                RecordMatterId = AssemblyJobs.First().RecordMatter.First().RecordMatterId,
                //Type
                SubmissionStatus = "Assembling"
            });

            AssemblyJobs.ForEach(m =>
            {
                _documentAssemblyManager.AssemblyManager(m);
            });

        }

        //recrusively save upload files in the repeat panel
        private void SaveUploadFileInRepeatPanel(JToken repeatItems, RecordSet recordSet, string accessToken)
        {
            foreach (JObject repeatItem in repeatItems.Children())
            {
                foreach (KeyValuePair<string, JToken> subkey in repeatItem)
                {
                    string SubKey = subkey.Key;
                    if (!string.IsNullOrEmpty(SubKey) && repeatItem[SubKey].Type == JTokenType.Array && repeatItem[SubKey].HasValues)
                    {
                        if (repeatItem[SubKey][0].Value<bool>("file") == true)
                        {
                            foreach (JToken file in (JArray)repeatItem[SubKey])
                            {
                                if (file.HasValues)
                                {
                                    FileDto FileDto = JsonConvert.DeserializeObject<FileDto>(file.ToString());
                                    _filesManager.SaveFile(new SaveFileDto()
                                    {
                                        RecordId = recordSet.RecordId,
                                        RecordMatterId = recordSet.RecordMatterId,
                                        RecordMatterItemGroupId = recordSet.RecordMatterItemGroupId,
                                        File = FileDto,
                                        AccessToken = accessToken
                                    });
                                }
                            }

                        }
                        else if (repeatItem[SubKey][0].Value<bool>("repeat") == true)
                        {
                            JToken r = repeatItem[SubKey];
                            SaveUploadFileInRepeatPanel(r, recordSet, accessToken);
                        }
                    }

                }
            }
            return;
        }

        //recrusively find if there is a repeat panel inside repeat panel
        private bool FindUploadFileinRepeat(bool hasFile, JToken repeatItems)
        {
            foreach (JObject repeatItem in repeatItems.Children())
            {
                foreach (KeyValuePair<string, JToken> subkey in repeatItem)
                {
                    string SubKey = subkey.Key;
                    if (!string.IsNullOrEmpty(SubKey) && repeatItem[SubKey].Type == JTokenType.Array && repeatItem[SubKey].HasValues)
                    {
                        if (repeatItem[SubKey][0].Value<bool>("file") == true)
                        {
                            hasFile = true;
                        }
                        else if (repeatItem[SubKey][0].Value<bool>("repeat") == true)
                        {
                            JToken r = repeatItem[SubKey];
                            hasFile = FindUploadFileinRepeat(hasFile, r);
                        }
                    }
                }
            }
            return hasFile;
        }


        private Boolean ValidateSubmission(JObject submission, Guid? entityId)
        {
            // EXAMPLE
            // string schemaJson = @"{
            //  'properties': {
            //    'approved': {'type': 'boolean'},
            //    'name': {'type': 'boolean'},
            //    'NumberTest': {
            //          'type': 'number',
            //          'minimum': 25,
            //          'maximum': 100
            //    }
            //  }
            //}";


            var result = true;
            string schemaJson = string.Empty;

            if (entityId != null)
            {
                var form = _formRepository.FirstOrDefault(e => e.Id == entityId);
                if (form != null)
                    schemaJson = form.RulesSchema;

                var app = _appRepository.FirstOrDefault(e => e.Id == entityId);
                if (app != null)
                    schemaJson = app.RulesSchema;
            }

            if (!string.IsNullOrEmpty(schemaJson))
            {
                try
                {
                    JSchema rulesSchema = JSchema.Parse(schemaJson);
                    result = submission.IsValid(rulesSchema);
                }
                catch
                {
                    // If Error reject test
                    result = false;
                }
            }

            return result;

        }

        //TODO refractor into extension or utility
        private string GetJArrayValue(JObject yourJArray, string key)
        {
            foreach (KeyValuePair<string, JToken> keyValuePair in yourJArray)
            {
                if (key == keyValuePair.Key)
                {
                    return keyValuePair.Value.ToString();
                }
            }

            return null;
        }


        ////TODO refractor into extension or utility
        //private string SetJArrayValue(JObject yourJArray, string key, string value)
        //{
        //    foreach (KeyValuePair<string, JToken> keyValuePair in yourJArray)
        //    {
        //        if (key == keyValuePair.Key)
        //        {
        //            return keyValuePair.Value.
        //        }
        //    }

        //    return null;
        //}

        // Gets MText for assembly job
        // Must be XML for Document Assembly engine in this format
        private string GetMtext(CreateOrEditAppJobDto job)
        {
            string result = "<MergeText>{0}</MergeText>";
            var mtext = _mergeTextRepository.GetAllIncluding().Where(i => i.EntityKey == job.User.ID.ToString() && i.EntityType == "User");

            if (mtext.Count() == 0)
            {
                mtext = _mergeTextRepository.GetAllIncluding().Where(i => i.TenantId == null && i.EntityType == "User");
            }

            //// Check User MText
            //if(mtext.Count() == 0)
            //    mtext = _mergeTextRepository.GetAllIncluding().Where(i => i.EntityKey == job.User.ID.ToString() && i.EntityType == "User");

            mtext.ToList().ForEach(mt =>
            {
                mt.MergeTextItems.ForEach(mti =>
                {
                    var order = 0;
                    mti.MergeTextItemValues.ForEach(mtiv =>
                    {
                        result = String.Format(result, "<" + mti.Name + " value=\"" + mtiv.Value + "\" key=\"" + mtiv.Key + "\"  order=\"" + order + "\" ></" + mti.Name + ">{0}");
                        order++;
                    });

                });
            });

            result = string.Format(result, string.Empty); // remove last {0}
            return result;
        }

        // Gets MText for assembly job
        // Must be XML for Document Assembly engine in this format
        private void SetBLNL(XElement data)
        {

            data.Descendants().Where(i => i.Name.LocalName.ToLower().EndsWith("mtext")).ToList().ForEach(n => {

                var NLname = n.Name.LocalName.ToLower().Replace("mtext", "NL");
                //XElement xNL = XElement.Parse("<" + NLname + ">" + n.Value.Replace(",","\r\n") + "</" + NLname + ">");
                XElement xNL = XElement.Parse("<" + NLname + "></" + NLname + ">");
                xNL.Value = n.Value.Replace(",", "\r\n");
                n.AddAfterSelf(xNL);

                var BLNLname = n.Name.LocalName.ToLower().Replace("mtext", "BLNL");
                var BLNLvalues = n.Value.Split(",");
                var BLNLvalue = string.Empty;
                var BLNSep = string.Empty;

                for (int i = 0; i < BLNLvalues.Length; i++)
                {
                    BLNSep = string.Empty;
                    if (i < BLNLvalues.Length - 2)
                    {
                        BLNSep = ", \r\n";
                    }
                    else if (i == BLNLvalues.Length - 2)
                    {
                        BLNSep = ", and \r\n";
                    }

                    BLNLvalue += BLNLvalues[i] + BLNSep;

                }

                BLNLvalue += ".";

                //XElement xBLNL = XElement.Parse("<" + BLNLname + ">" + BLNLvalue + "</" + BLNLname + ">");
                XElement xBLNL = XElement.Parse("<" + BLNLname + "></" + BLNLname + ">");
                xBLNL.Value = BLNLvalue;
                n.AddAfterSelf(xBLNL);

            });

        }

        private string GetFieldValue(string data, string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                var result = input;
                var jdata = JsonConvert.DeserializeObject<JObject>(data);
                Regex Pattern = new Regex("{.*?}", RegexOptions.IgnoreCase);
                var matches = Pattern.Matches(input);
                matches.ToList().ForEach(i1 =>
                {
                    result = StringUtility.ReplaceBracketValuesJSON(i1, jdata, result);
                });
                return string.IsNullOrEmpty(result) ? input : result;
            }
            else
            {
                return "Default";
            }
        }

        //private Task<Task> PreprocessJob(AppJob job, dynamic JSONObject)
        //{
        //    //Action<CreateOrEditAppJobDto> X = AssemblyJobs;
        //    //return Task.Run(async() =>
        //    //{

        //    //});

        //}

        private string Serialize<T>(T o)
        {
            var attr = o.GetType().GetCustomAttribute(typeof(JsonObjectAttribute)) as JsonObjectAttribute;
            var jv = JValue.FromObject(o);
            return new JObject(new JProperty("Data", jv)).ToString();
        }

        // TODO move to a helper class
        public static bool BooleanNullToFalse(dynamic value)
        {
            return value ?? false;
        }

        public static decimal DecimalNullToValue(dynamic value)
        {
            return value ?? 0;
        }

        public static long LongNullToValue(dynamic value)
        {
            return value ?? 0;
        }

        public static void SubDataFormat(JToken mainKey, JToken visitKey, JToken visitData, ref ExpandoObject desiredFormat)
        {
            switch (visitData.Type)
            {
                // Flattens a radio / yn control
                // From {"YesNoFieldName" : { "ValueA" : true, "ValueB" : false } 
                // To   {"YesNoFieldName" : "ValueA"}
                case JTokenType.Object:

                    //JObject CurrentData = JsonConvert.DeserializeObject<JObject>(visitData.ToString(), new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
                    JObject CurrentData = JsonConvert.DeserializeObject<JObject>(visitData.ToString());
                    var objvalue = string.Empty;
                    bool requireEmptyValue = true;

                    foreach (var itemkey in CurrentData)
                    {

                        foreach (var key in CurrentData)
                        {

                            string CurrentKey = key.Key;

                            //bool mybool = false;


                            var keyval = Convert.ToString(key.Value).ToLower();
                            if (keyval == "true" || keyval == "false")
                            {
                                visitData[key.Key] = Convert.ToString(visitData[key.Key]).ToLower();
                            }

                            SubDataFormat(mainKey, key.Key, visitData[key.Key], ref desiredFormat);

                            // If a checkbox is not selected we need to create an empty default value of ""
                            if ((key.Key != "true" && key.Key != "false") || ((key.Key == "true" || key.Key == "false") && (keyval == "true")))
                            {
                                requireEmptyValue = false;
                            }

                        }

                    }

                    // Do we need to create the key with an empty value?
                    // If has a true and a false and niether is selected then create '' value for key
                    // If a checkbox is not slected we need to create an empty default value of ""
                    if (requireEmptyValue)
                    {
                        SubDataFormat(mainKey, mainKey, "", ref desiredFormat);
                    }


                    //SubDataFormat(mainKey, visitKey, objvalue, ref desiredFormat);

                    break;
                case JTokenType.Array:
                    //var x = visitData;
                    //var y = visitData[0];
                    //var z = visitData[0]["repeat"];
                    if (visitData.HasValues)
                    {
                        if (visitData[0].Value<bool>("repeat") == true || visitData[0].Value<bool>("file") == true)
                        {
                            JArray rpt = (JArray)visitData;
                            ((IDictionary<string, object>)desiredFormat)[visitKey.ToString()] = new Dictionary<string, object>();
                            //dynamic subDesiredFormat = new ExpandoObject();
                            //var saveKey = visitKey.ToString();
                            var RptData = new List<object>();
                            //subDesiredFormat.saveKey = new object[rpt.Count];
                            for (var i = 0; i < rpt.Count; i++)
                            {
                                var subRepeatSection = new ExpandoObject();
                                /* as IDictionary<string, Object>*/
                                //;
                                foreach (var subToken in visitData[i])
                                {
                                    if (subToken.HasValues)
                                    {
                                        //JToken CurrentToken = JToken.Parse(subToken.ToString());
                                        //foreach (var subKey in CurrentToken)
                                        //{                                        
                                        string CurrentKey = ((JProperty)subToken).Name;
                                        SubDataFormat(CurrentKey, CurrentKey, visitData[i][CurrentKey], ref subRepeatSection);
                                        //}
                                    }

                                    //subDesiredFormat.saveKey = new { subRepeatSection }; //.Add("NewProp", string.Empty);
                                }
                                RptData.Add(subRepeatSection);
                            }
                        ((IDictionary<string, object>)desiredFormat)[visitKey.ToString()] = RptData;
                        }
                        else
                        {
                            JArray rpt = (JArray)visitData;
                            for (var i = 0; i < rpt.Count; i++)
                            {
                                foreach (var subToken in visitData[i])
                                {
                                    if (subToken.HasValues)
                                    {
                                        //JToken CurrentToken = JToken.Parse(subToken.ToString());
                                        //foreach (var subKey in CurrentToken)
                                        //{                                        
                                        string CurrentKey = ((JProperty)subToken).Name;
                                        SubDataFormat(mainKey, CurrentKey, visitData[i][CurrentKey], ref desiredFormat);
                                    }
                                }
                            }
                        }
                    }
                    break;
                default:

                    var data = visitData;
                    // Convert to lowercase if a bool
                    if (visitData.Type is Newtonsoft.Json.Linq.JTokenType.Boolean)
                    {
                        data = visitData.ToString().ToLower();
                    }

                    if (mainKey.ToString() == visitKey.ToString())
                    {
                        ((IDictionary<string, object>)desiredFormat)[mainKey.ToString()] = data + "";
                    }
                    else
                    {
                        var combineKey = mainKey + "_" + visitKey;

                        ((IDictionary<string, object>)desiredFormat)[combineKey] = data + "";

                        if (visitKey.ToString().ToLower() == "value")
                        {
                            ((IDictionary<string, object>)desiredFormat)[mainKey.ToString()] = data + "";
                        }

                    }
                    break;
            }

            //if (visitData.HasValues)
            //{
            //    JObject CurrentData = JObject.Parse(visitData.ToString());
            //    foreach (var key in CurrentData)
            //    {
            //        string CurrentKey = key.Key;
            //        SubDataFormat(mainKey, CurrentKey, visitData[CurrentKey], ref desiredFormat);
            //    }
            //}
            //else if ()
            //{

            //}
            //else
            //{
            //    if (mainKey.ToString() == visitKey.ToString())
            //    {
            //        ((IDictionary<string, object>)desiredFormat)[mainKey.ToString()] = visitData + "";
            //    }
            //    else
            //    {
            //        var combineKey = mainKey + "_" + visitKey;
            //        ((IDictionary<string, object>)desiredFormat)[combineKey] = visitData + "";
            //    }
            //}
        }


    }
}