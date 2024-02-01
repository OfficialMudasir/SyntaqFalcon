using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NUglify.Helpers;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.Apps;
using Syntaq.Falcon.Apps.Dtos;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Authorization.Roles;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Common.Dto;
using Syntaq.Falcon.Configuration;
using Syntaq.Falcon.Documents;
using Syntaq.Falcon.Documents.Models;
using Syntaq.Falcon.EntityFrameworkCore.Repositories;
using Syntaq.Falcon.Files;
using Syntaq.Falcon.Files.Dtos;
using Syntaq.Falcon.Folders;
using Syntaq.Falcon.Folders.Dtos;
using Syntaq.Falcon.Forms.Dtos;
using Syntaq.Falcon.Forms.Exporting;
using Syntaq.Falcon.Forms.Models;
using Syntaq.Falcon.MultiTenancy;
using Syntaq.Falcon.Projects;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Utility;
using Syntaq.Falcon.Vouchers;
using Syntaq.Falcon.Web;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Syntaq.Falcon.Projects.ProjectConsts;
using static Syntaq.Falcon.Records.RecordMatterContributorConsts;
using JsonDiffer;
using Syntaq.Falcon.EntityVersionHistories;
using Abp.Timing.Timezone;
using Syntaq.Falcon.Notifications;
using Syntaq.Falcon.Authorization.Users.Dto;
using Syntaq.Falcon.Notifications.Dto;
using Syntaq.Falcon.Organizations;
using Syntaq.Falcon.Organizations.Dto;
using Abp.Notifications;
using Abp;
using Syntaq.Falcon.Projects.Dtos;
using NPOI.SS.Formula.Functions;
using Hangfire.Storage;
using Syntaq.Falcon.Submissions.Dtos;
using Syntaq.Falcon.Submissions;
using System.Text.Json.Nodes;
using Stripe;
using System.IO;
using PayPalCheckoutSdk.Core;
using Microsoft.Extensions.Configuration;

namespace Syntaq.Falcon.Forms
{
    [EnableCors("AllowAll")]
    public class FormsAppService : FalconAppServiceBase, IFormsAppService
    {
        private enum FormType { UserForm = 1, PaymentForm };

        private readonly ACLManager _ACLManager;
        private readonly IRepository<ACL, int> _aclRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly TenantManager _tenantManager;
        private readonly FolderManager _folderManager;
        private readonly RecordManager _recordManager;
        private readonly IRepository<Apps.App, Guid> _appRepository;
        private readonly IRepository<Form, Guid> _formRepository;
        private readonly IRepository<ProjectRelease, Guid> _projectReleaseRepository;

        private readonly ProjectManager _projectManager;

        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IRepository<FormRule, Guid> _formRuleRepository;
        private readonly IRepository<RecordMatterContributor, Guid> _recordMatterContributorRepository;

        private readonly ICustomFormsRepository _customFormRepository;

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IFormsExcelExporter _formsExcelExporter;
        private readonly IDocumentsAppService _documentAppService;
        private readonly IRepository<Folder, Guid> _folderRepository;
        private readonly IRepository<AppJob, Guid> _appJobRepository;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;

        private readonly IRepository<Record, Guid> _recordRepository;
        private readonly IRepository<RecordMatterItem, Guid> _recordMatterItemRepository;
        private readonly IRepository<RecordMatter, Guid> _recordMatterRepository;

        readonly ISettingDefinitionManager _settingDefinitionManager;

        private readonly IAuditingStore _auditLogRepository;
        private readonly IOptions<NZBNConnection> _NZBNConnection;
        private readonly RoleManager _roleManager;
        private readonly FilesManager _filesManager;
        private readonly IRepository<EntityVersionHistory, Guid> _entityVersionHistoryRepository;
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly SubmissionManager _submissionManager;
        public bool IsSaveHit { get; set; }

        private readonly IConfiguration _configuration;
        private readonly int _jwtExpiry= 365;

        private readonly IOptions<JSONWebToken> _JSONWebToken;

        public FormsAppService(
                SubmissionManager submissionManager,
                ITimeZoneConverter timeZoneConverter,
                FilesManager filesManager,
                ACLManager aclManager,
                RoleManager roleManager,
                IRepository<User, long> userRepository,
                TenantManager tenantManager,
                FolderManager folderManager,
                RecordManager recordManager,
                ProjectManager projectManager,
                IRepository<Apps.App, Guid> appRepository,
                IRepository<Form, Guid> formRepository,

                IRepository<ProjectRelease, Guid> projectReleaseRepository,
                IRepository<Project, Guid> projectRepository,
                IRepository<RecordMatterContributor, Guid> recordMatterContributorRepository,
                IRepository<FormRule, Guid> formRuleRepository,
                ICustomFormsRepository customformRepository,
                IUnitOfWorkManager unitOfWorkManager,
                IFormsExcelExporter formsExcelExporter,
                IDocumentsAppService documentAppService,
                IRepository<Folder, Guid> folderRepository,
                IRepository<AppJob, Guid> appJobRepository,
                IRepository<ACL, int> aclRepository,
                IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
                IRepository<Record, Guid> recordRepository,
                IRepository<RecordMatterItem, Guid> recordMatterItemRepository,
                IRepository<RecordMatter, Guid> recordMatterRepository,
                ISettingDefinitionManager settingDefinitionManager,
                IRepository<EntityVersionHistory, Guid> entityVersionHistoryRepository,
                IAuditingStore auditLogRepository,

                IOptions<NZBNConnection> NZBNConnection,
                IVouchersAppService voucherAppService,
                IOptions<JSONWebToken> JSONWebToken,
                IConfiguration configuration
            )
        {
            _submissionManager = submissionManager;
            _filesManager = filesManager;
            _roleManager = roleManager;
            _ACLManager = aclManager;
            _aclRepository = aclRepository;
            _userRepository = userRepository;
            _tenantManager = tenantManager;
            _folderManager = folderManager;
            _recordManager = recordManager;
            _projectManager = projectManager;
            _appRepository = appRepository;
            _formRepository = formRepository;

            _projectReleaseRepository = projectReleaseRepository;
            _projectRepository = projectRepository;
            _recordMatterContributorRepository = recordMatterContributorRepository;
            _formRuleRepository = formRuleRepository;
            _customFormRepository = customformRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _formsExcelExporter = formsExcelExporter;
            _documentAppService = documentAppService;
            _folderRepository = folderRepository;
            _appJobRepository = appJobRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _recordRepository = recordRepository;
            _recordMatterItemRepository = recordMatterItemRepository;
            _recordMatterRepository = recordMatterRepository;
            _settingDefinitionManager = settingDefinitionManager;

            _auditLogRepository = auditLogRepository;
            _timeZoneConverter = timeZoneConverter;

            _NZBNConnection = NZBNConnection;
            _entityVersionHistoryRepository = entityVersionHistoryRepository;

            _configuration = configuration;
            //_jwtExpiry = _configuration.GetValue<int>("JSONWebToken:Expiry", 365);
            _jwtExpiry = JSONWebToken.Value.Expiry;
            _JSONWebToken = JSONWebToken;

        }

        [AbpAuthorize(AppPermissions.Pages_Forms)]
        public async Task<PagedResultDto<FormFolderDto>> GetAll(GetAllFormsInput input)
        {

            Guid userRootFolderId = _ACLManager.FetchUserRootFolder((long)AbpSession.UserId, "F");
            Guid parentFolderId = (string.IsNullOrEmpty(input.Id) || input.Id == "00000000-0000-0000-0000-000000000000") ? userRootFolderId : new Guid(input.Id);
            input.Id = input.Id == "00000000-0000-0000-0000-000000000000" || input.Id == null ? userRootFolderId.ToString() : input.Id;

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = Guid.Parse(input.Id), UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {

                input.Filter = input.Filter?.Trim();

                List<FormFolderDto> dtoResult = new List<FormFolderDto>();

                // If Doing a filter Search need all the Folders you are searching
                IQueryable<Guid> folderIds = Enumerable.Empty<Guid>().AsQueryable();
                if (!string.IsNullOrWhiteSpace(input.Filter))
                {
                    folderIds = GetChildFolderIds(new Guid(input.Id));
                    folderIds = folderIds.Concat(new Guid[] { new Guid(input.Id) });
                }

                var forms = from s in _customFormRepository.GetAllForIndex(AbpSession.UserId)
                    .Where(e => string.IsNullOrWhiteSpace(input.Filter) ? true : (e.Name.ToLower().Contains(input.Filter.ToLower()) || e.Description.ToLower().Contains(input.Filter.ToLower())) && e.CreatorUserId == AbpSession.UserId)
                    .Where(e => string.IsNullOrWhiteSpace(input.Filter) ? e.FolderId == parentFolderId : folderIds.Contains(e.FolderId))
                    .Where(e => e.TenantId == AbpSession.TenantId)
                    .ToList()
                            group s by s.OriginalId into g
                            select new Form
                            {
                                CreationTime = g.First().CreationTime,
                                CreatorUserId = g.First().CreatorUserId,
                                CurrentVersion = g.First().CurrentVersion,
                                DeleterUserId = g.First().DeleterUserId,
                                DeletionTime = g.First().DeletionTime,
                                Description = g.Any(x => x.CurrentVersion == x.Version) == true ? g.First(x => x.CurrentVersion == x.Version).Description : g.First().Description,
                                FolderId = g.First().FolderId,
                                Id = g.First().Id,
                                IsDeleted = g.First().IsDeleted,
                                LastModificationTime = g.First().LastModificationTime,
                                LastModifierUserId = g.First().LastModifierUserId,
                                OriginalId = g.First().OriginalId,
                                Name = g.Any(x => x.CurrentVersion == x.Version) == true ? g.First(x => x.CurrentVersion == x.Version).Name : g.First().Name,
                                TenantId = g.First().TenantId,
                                Version = g.Max(x => x.Version),
                                VersionName = g.First().VersionName
                            };

                var folders = (_folderRepository.GetAll()
                    .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter))
                    .Where(e => e.ParentId == parentFolderId)).ToList();

                // Shared Forms

                // If in users root folder then show all when directly shared not via fodler sharing
                // If in shared folder only show forms in folder (which are retrieved via the GetAllForIndex above)

                var sharedForms = from s in _customFormRepository.GetSharedForms(AbpSession.UserId)
                    .Where(e => string.IsNullOrWhiteSpace(input.Filter) ? true : e.Name.ToLower().Contains(input.Filter.ToLower()) || e.Description.ToLower().Contains(input.Filter.ToLower()))
                    .Where(e => string.IsNullOrWhiteSpace(input.Filter) && parentFolderId != userRootFolderId ? e.FolderId == parentFolderId : true)
                    .Where(e => e.TenantId == AbpSession.TenantId)
                    .ToList()
                                  group s by s.OriginalId into g
                                  select new Form
                                  {
                                      CreationTime = g.First().CreationTime,
                                      CreatorUserId = g.First().CreatorUserId,
                                      CurrentVersion = g.First().CurrentVersion,
                                      DeleterUserId = g.First().DeleterUserId,
                                      DeletionTime = g.First().DeletionTime,
                                      Description = g.Any(x => x.CurrentVersion == x.Version) == true ? g.First(x => x.CurrentVersion == x.Version).Description : g.First().Description,
                                      FolderId = g.First().FolderId,
                                      Id = g.First().Id,
                                      IsDeleted = g.First().IsDeleted,
                                      LastModificationTime = g.First().LastModificationTime,
                                      LastModifierUserId = g.First().LastModifierUserId,
                                      OriginalId = g.First().OriginalId,
                                      Name = g.Any(x => x.CurrentVersion == x.Version) == true ? g.First(x => x.CurrentVersion == x.Version).Name : g.First().Name,
                                      TenantId = g.First().TenantId,
                                      Version = g.Max(x => x.Version),
                                      VersionName = g.First().VersionName
                                  };

                forms = forms.Concat(sharedForms).Distinct();

                // Get Shared Folders if in the rrot folder of the user
                if (parentFolderId == userRootFolderId)
                {

                    var sharedFolders = (from folder in _folderRepository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter))
                                         join acl in _aclRepository.GetAll() on folder.Id equals acl.EntityID
                                         where acl.CreatorUserId != AbpSession.UserId && acl.UserId == AbpSession.UserId && folder.Type == "F"
                                         select folder).ToList();

                    sharedFolders = (sharedFolders.Concat(from folder in _folderRepository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter))
                                                          join acl in _aclRepository.GetAll() on folder.Id equals acl.EntityID
                                                          join ut in _userOrganizationUnitRepository.GetAll() on acl.OrganizationUnitId equals ut.OrganizationUnitId
                                                          where ut.UserId == AbpSession.UserId && folder.Type == "F"
                                                          select new Folder
                                                          {
                                                              OrganizationUnitId = ut.OrganizationUnitId,
                                                              ACLRole = acl.Role,
                                                              Id = folder.Id,
                                                              Name = folder.Name,
                                                              Description = folder.Description,
                                                          })).ToList();

                    folders = folders.Concat(sharedFolders).Distinct().ToList();
                }

                var formcnt = forms.Count();
                var foldercnt = folders.Where(i => i.Name != "Your Forms").Count();

                //ACLCheckDto aCLCheckDto = new ACLCheckDto(){UserId = AbpSession.UserId};
                folders.Where(i => i.Name != "Your Forms").Skip(input.SkipCount).Take(input.MaxResultCount).OrderBy(i => i.Name).ToList().ForEach(i =>
                {
                    //aCLCheckDto.EntityId = i.Id;
                    var Result = new FormFolderDto()
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Description = i.Description,
                        LastModified = i.LastModificationTime == null ? i.CreationTime : (DateTime)i.LastModificationTime,
                        Type = "Folder",
                        UserACLPermission = i.ParentId == parentFolderId ? ACLResult.Permission : _ACLManager.FetchRole(new ACLCheckDto() { EntityId = i.Id, UserId = AbpSession.UserId, OrgId = i.OrganizationUnitId != null ? i.OrganizationUnitId : null })
                    };
                    dtoResult.Add(Result);
                });

                var formsToSkip = input.SkipCount == 0 ? input.SkipCount : (input.SkipCount - dtoResult.Count() - foldercnt) + dtoResult.Count();
                var formsToTake = input.MaxResultCount - dtoResult.Count;

                //check login user is admin or not				

                /////////////////////////////////////////////////////////////////
                // Fails on User Testing Tenant / admin user in Test environment
                // Bruce
                /////////////////////////////////////////////////////////////////

                //var r = AbpSession.UserId == null ? null : await _roleManager.GetRoleByIdAsync((long)AbpSession.UserId);
                //List<Guid> containSharedProject = new List<Guid>();
                ////if login user is admin, search if this form is shared to other tenant
                //if (r != null && r.Name == "Admin")
                //{
                //	forms.ForEach(o => {
                //		if (hasSteps(o.Id))
                //		{
                //			containSharedProject.Add(o.Id);
                //		}
                //	});
                //}

                List<Guid> containSharedProject = new List<Guid>();
                var user = UserManager.GetUserById((long)AbpSession.UserId);

                if (await UserManager.IsInRoleAsync(user, StaticRoleNames.Host.Admin))
                {
                    forms.ForEach(o =>
                    {
                        if (hasSteps(o.Id))
                        {
                            containSharedProject.Add(o.Id);
                        }
                    });
                };


                forms.OrderByDescending(i => i.LastModificationTime).Skip(formsToSkip).Take(formsToTake).ToList().ForEach(i =>
                {
                    var Result = new FormFolderDto()
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Description = i.Description,
                        Version = i.Version,
                        CurrentVersion = i.CurrentVersion,
                        OriginalId = i.OriginalId,
                        LastModified = i.CreationTime,
                        Type = "Form",
                        UserACLPermission = i.FolderId == parentFolderId ? ACLResult.Permission : _ACLManager.FetchRole(new ACLCheckDto() { EntityId = i.OriginalId, UserId = AbpSession.UserId, OrgId = i.OrganizationUnitId != null ? i.OrganizationUnitId : null }),// take the permission of the parent (max permission)
                        Shared = containSharedProject.Contains(i.Id),
                    };
                    dtoResult.Add(Result);
                });


                IQueryable<FormFolderDto> iQformFolders;
                iQformFolders = dtoResult.AsQueryable();
                iQformFolders = iQformFolders.OrderBy("type asc").ThenBy(input.Sorting ?? "type asc");//.PageBy(input);
                dtoResult = iQformFolders.ToList();

                int totalCount = formcnt + foldercnt;

                return new PagedResultDto<FormFolderDto>(totalCount, dtoResult);
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }
        }

        //Get All ChildFolerId's
        private IQueryable<Guid> GetChildFolderIds(Guid parentId)
        {

            IQueryable<Guid> result = Enumerable.Empty<Guid>().AsQueryable();

            var fIds = _folderRepository.GetAll().Where(f => f.ParentId == parentId).Select(f => f.Id).ToList();

            foreach (Guid id in fIds)
            {
                if (_folderRepository.GetAll().Any(f => f.ParentId == id))
                {
                    result = result.Concat(GetChildFolderIds(id));
                }
            }

            result = result.Concat(fIds);
            return result;
        }

        private Boolean hasSteps(Guid projecttemplateId)
        {
            try
            {
                var acls = _aclRepository.GetAllIncluding(i => i.User, i => i.OrganizationUnit)
                        .Where(i =>
                            i.EntityID == projecttemplateId &&
                            i.CreatorUserId != i.UserId &&
                            i.TargetTenantId != null
                        ).ToList();

                var result = (acls.Count > 0);
                return result;
            }
            catch
            {
                return false;
            }


        }

        //[AbpAuthorize(AppPermissions.Pages_Forms)]
        public async Task<PagedResultDto<FormFolderDto>> GetAllShared(GetAllFormsInput input)
        {

            // Get ID's of projects shared with this Tenant
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            var formids = _ACLManager.GetAllSharedEntities(AbpSession.TenantId, "Form");

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = Guid.Parse(input.Id), UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {
                List<FormFolderDto> dtoResult = new List<FormFolderDto>();

                var forms = from s in _customFormRepository.GetAllForIndex(AbpSession.UserId)
                    .Where(e => string.IsNullOrWhiteSpace(input.Filter) ? true : e.Name.ToLower().Contains(input.Filter.ToLower()) || e.Description.ToLower().Contains(input.Filter.ToLower()))
                    .Where(e => formids.Contains(e.Id))
                    .ToList()
                            group s by s.OriginalId into g
                            select new Form
                            {
                                CreationTime = g.First().CreationTime,
                                CreatorUserId = g.First().CreatorUserId,
                                CurrentVersion = g.First().CurrentVersion,
                                DeleterUserId = g.First().DeleterUserId,
                                DeletionTime = g.First().DeletionTime,
                                Description = g.Any(x => x.CurrentVersion == x.Version) == true ? g.First(x => x.CurrentVersion == x.Version).Description : g.First().Description,
                                FolderId = g.First().FolderId,
                                Id = g.First().Id,
                                IsDeleted = g.First().IsDeleted,
                                LastModificationTime = g.First().LastModificationTime,
                                LastModifierUserId = g.First().LastModifierUserId,
                                OriginalId = g.First().OriginalId,
                                Name = g.Any(x => x.CurrentVersion == x.Version) == true ? g.First(x => x.CurrentVersion == x.Version).Name : g.First().Name,
                                TenantId = g.First().TenantId,
                                Version = g.Max(x => x.Version),
                                VersionName = g.First().VersionName
                            };

                var formcnt = forms.Count();

                var formsToSkip = input.SkipCount == 0 ? input.SkipCount : (input.SkipCount - dtoResult.Count()) + dtoResult.Count();
                var formsToTake = input.MaxResultCount - dtoResult.Count;

                forms.OrderByDescending(i => i.LastModificationTime).Skip(formsToSkip).Take(formsToTake).ToList().ForEach(i =>
                {

                    var tenancyName = "Host";
                    MultiTenancy.Tenant tenant = null;
                    if (i.TenantId != null)
                    {
                        tenant = TenantManager.GetById((int)i.TenantId);
                        if (tenant != null)
                            tenancyName = tenant.TenancyName;
                    }

                    var acl = _aclRepository.GetAll().FirstOrDefault(a => a.EntityID == i.Id && a.TargetTenantId == AbpSession.TenantId);

                    var Result = new FormFolderDto()
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Description = i.Description,
                        Version = i.Version,
                        CurrentVersion = i.CurrentVersion,
                        OriginalId = i.OriginalId,
                        LastModified = i.CreationTime,
                        Type = "Form",
                        UserACLPermission = "View",  // take the permission of the parent (max permission)
                        Accepted = acl.Accepted,
                        TenancyName = tenancyName
                    };
                    dtoResult.Add(Result);
                });

                IQueryable<FormFolderDto> iQformFolders;
                iQformFolders = dtoResult.AsQueryable();
                iQformFolders = iQformFolders.OrderBy("type asc").ThenBy(input.Sorting ?? "type asc");//.PageBy(input);
                dtoResult = iQformFolders.ToList();

                int totalCount = formcnt;

                return new PagedResultDto<FormFolderDto>(totalCount, dtoResult);
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }
        }

        public async Task<List<GetFormForView>> GetVersionHistory(EntityDto<Guid> input)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = input.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
                var filteredTemplates = _formRepository.GetAll().Where(i => i.OriginalId == input.Id).OrderByDescending(i => i.Version);
                var query = (from o in filteredTemplates select new GetFormForView() { Form = ObjectMapper.Map<FormDto>(o) });
                return new List<GetFormForView>(
                    query.ToList()
                );
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

        public async Task<string> GetSchema(EntityDto<Guid> input, string Flag = "Build", int Version = 0)
        {
            //Form form = await _formRepository.GetAsync(input.Id);
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            // Authed is this Form is in this tenant or has a tenant acl
            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = input.Id, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId, AccessToken = string.Empty });

            if (ACLResult.IsAuthed)
            {
                Form form = await GetFormByVersion(input.Id, Version);
                if (form.IsEnabled != false)
                {
                    form.Schema = CheckingExistingButtons(JObject.Parse(form.Schema));//Yichao
                    switch (Flag)
                    {
                        case "Load":
                            if (string.IsNullOrEmpty(form.Schema)) { return form.Schema; }
                            if (!form.Schema.Contains("\"type\": \"nestedform\"") && !form.Schema.Contains("\"type\":\"nestedform\"")) { return form.Schema; }
                            return await GetEmbeddedSchemas(JObject.Parse(form.Schema));
                        case "Build":
                        default:
                            return form.Schema;
                    }
                }
                else
                {
                    return "{\"type\":\"form\",\"display\":\"form\",\"components\":[{\"title\":\"Disabled Form\",\"key\":\"notesHelpEditor\",\"htmlcontent\":\"Sorry! This Form is currently disabled. Please contact your system administrator.\",\"widthslider\":\"12\",\"offsetslider\":\"0\",\"type\":\"helpnotes\",\"input\":false,\"tableView\":false,\"label\":\"Notes /Help editor\"}]}";
                }
            }
            else
            {
                return "{\"type\":\"form\",\"display\":\"form\",\"components\":[{\"title\":\"Permission Denied\",\"key\":\"notesHelpEditor\",\"htmlcontent\":\"\",\"widthslider\":\"12\",\"offsetslider\":\"0\",\"type\":\"helpnotes\",\"input\":false,\"tableView\":false,\"label\":\"Notes /Help editor\"}]}";
            }

        }
        // Yichao
        private string CheckingExistingButtons(JObject formSchema)
        {
            var buttonPaths = formSchema.DescendantsAndSelf().OfType<JProperty>()
                .Where(p => p.Name == "key" && (p.Value.ToString() == "save" || (p.Name == "key" && p.Value.ToString() == "submit")))
                .ToArray();
            foreach (var path in buttonPaths)
            {
                string buttonPath = path.Parent.Path;
                formSchema.SelectToken(buttonPath).Remove();
            }
            return JsonConvert.SerializeObject(formSchema);
        }

        //Depreciated
        // This method can be used to generate the field name list for RulesBuilder.
        private async Task<string> GetEmbeddedSchemas(JObject formSchema)
        {
            var path = formSchema.DescendantsAndSelf().OfType<JProperty>().Where(p => p.Name == "type" && p.Value.ToString() == "nestedform").First();
            string pathString = path.Parent.Path;
            while (!string.IsNullOrEmpty(pathString))
            {
                try
                {
                    var subForm = await GetSchema(new EntityDto<Guid> { Id = Guid.Parse(formSchema.SelectToken(pathString)["FormId"].ToString()) }, "Load");
                    List<JToken> components = JObject.Parse(subForm).SelectToken("components").ToList();
                    var FNS = formSchema.SelectToken(pathString)["FNSubstitute"].ToList();
                    for (int i = components.Count - 1; i >= 0; i--)
                    {
                        if (components[i]["key"].ToString() == "submit" || components[i]["key"].ToString() == "save")
                        {
                            continue;
                        }
                        foreach (JObject fns in FNS)
                        {
                            string pattern = fns["FNPattern"].ToString();
                            string replacement = fns["FNReplacement"].ToString();
                            if (!string.IsNullOrEmpty(pattern) && !string.IsNullOrEmpty(replacement))
                            {
                                string temp = components[i].ToString().Replace(pattern, replacement);
                                components[i] = JToken.Parse(temp);

                            }
                        }
                        if (i == 0)
                        {
                            formSchema.SelectToken(pathString).Replace(components[i]);
                        }
                        else
                        {
                            formSchema.SelectToken(pathString).AddAfterSelf(components[i]);
                        }
                    }
                    pathString = "";
                    if (formSchema.DescendantsAndSelf().OfType<JProperty>().Any(p => p.Name == "type" && p.Value.ToString() == "nestedform"))
                    {
                        var newPath = formSchema.DescendantsAndSelf().OfType<JProperty>().Where(p => p.Name == "type" && p.Value.ToString() == "nestedform").First();
                        pathString = newPath.Parent.Path;
                    }

                }
                catch (Exception e) { }
            }
            return JsonConvert.SerializeObject(formSchema);
        }

        public async Task<bool> SetSchema(string Id, string Schema)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = new Guid(Id), UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
                Form form = await _formRepository.GetAsync(Guid.Parse(Id));
                var dataA = form.Schema;
                form.Schema = Schema;
                var jDiff = GetDataDiff(Schema, dataA);
                await _formRepository.UpdateAsync(form);

                //if updated form is live version, update the version history
                if (form.Version == form.CurrentVersion)
                {
                    var rma = new EntityVersionHistory()
                    {
                        Data = jDiff == null ? "{}" : jDiff.ToString(),
                        Name = form.Name,
                        Description = form.LastModificationTime == null && form.Version == 1 ? "Create a New Form" : "Update live Version",
                        Version = form.Version,
                        VersionName = form.VersionName,
                        PreviousVersion = form.CurrentVersion, //live version	
                        EntityId = form.Id,
                        UserId = AbpSession.UserId,
                        TenantId = AbpSession.TenantId,
                        PreviousData = dataA,
                        NewData = Schema,
                        Type = "Form",
                    };

                    await _entityVersionHistoryRepository.InsertAsync(rma);

                    ACL aCL = new ACL()
                    {
                        UserId = AbpSession.UserId,
                        TenantId = AbpSession.TenantId,
                        EntityID = rma.Id,
                        Type = "EntityVersionHistory",
                        Role = "O",
                    };
                    await _ACLManager.AddACL(aCL);

                }

                return true;
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

        public async Task<bool> SetRulesSchema(string Id, string Schema)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = new Guid(Id), UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {
                // Purposed for Both Apps and Forms  
                Form form = _formRepository.FirstOrDefault(e => e.Id == Guid.Parse(Id));
                if (form != null)
                {
                    form.RulesSchema = Schema;
                    await _formRepository.UpdateAsync(form);
                }

                Syntaq.Falcon.Apps.App app = _appRepository.FirstOrDefault(e => e.Id == Guid.Parse(Id));
                if (app != null)
                {
                    app.RulesSchema = Schema;
                    await _appRepository.UpdateAsync(app);
                }

                return true;
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

        //Needs Authorization?
        //TODO convert to DTO TASK output model
        public List<FormListDto> GetFormsList(string Flag = "normal", string Version = "all")
        {

            //form entityid not in the acl list, but it's parent folder id in the acl list
            //We need the ACLs for this use for all Folders (Form Folders)
            IEnumerable<Guid> TUserACLs = _ACLManager.FetchAllUserACLs(new GetAllACLsInput() { UserId = (long)AbpSession.UserId, EntityFilter = "Form" }).ToList().Select(a => a.EntityID);
            IEnumerable<Guid> TFolderACLs = _ACLManager.FetchAllUserACLs(new GetAllACLsInput() { UserId = (long)AbpSession.UserId, EntityFilter = "Folder" }).ToList().Select(a => a.EntityID);

            // Fetch ACLS disables tenant filter must turn it back on
            _unitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant);

            // Select all Forms Where the FolderId is in the TFolderACRs list
            var result = _formRepository.GetAll()
                    .Where(f => f.IsEnabled && f.Schema != null && (TFolderACLs.Any(fa => fa == f.FolderId) || TUserACLs.Any(a => a == f.OriginalId)))
                    .WhereIf(Flag == "normal", f2 => !f2.Schema.Contains("\"display\":\"wizard\""))
                    .WhereIf(Version != "all", f => f.Version == f.CurrentVersion)
                    .Select(f => new FormListDto { value = Convert.ToString(f.Id), label = Version == "all" ? $"{f.Name} [ver.{f.Version}]" : f.Name, Mtext = f.Description })
                    .ToList();

            result = result.Distinct().OrderBy(f => f.label).ToList();

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Forms_Edit)]
        public async Task<GetFormForEditOutput> GetFormForEdit(GetFormForViewDto input)
        {
            var formId = input.Id ?? input.OriginalId;
            if (!formId.HasValue)
                throw new UserFriendlyException("Permission Denied");

            if (_ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = (Guid)formId, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId }).IsAuthed
            )
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

                Form form = null;
                int.TryParse(input.Version, out int VersionInt);
                if (VersionInt != 0 && VersionInt.GetType() == typeof(int))
                {
                    form = await GetFormByVersion((Guid)input.OriginalId, VersionInt);
                }
                else if (!string.IsNullOrEmpty(input.Version) && input.Version.GetType() == typeof(string))
                {
                    form = await GetFormByVersion((Guid)input.OriginalId, null);
                }
                else
                {
                    try
                    {
                        form = await GetFormByVersion((Guid)input.OriginalId, null);
                    }
                    catch (Exception ex)
                    {
                        throw new UserFriendlyException("Fetching Form failed because: " + ex);
                    }
                }

                var Form = new CreateOrEditFormDto
                {
                    Id = form.Id,
                    Name = form.Name,
                    VersionName = form.VersionName,
                    Description = form.Description,
                    PaymentEnabled = form.PaymentEnabled,
                    PaymentAmount = form.PaymentAmount,
                    PaymentCurrency = form.PaymentCurrency,
                    PaymentProcess = form.PaymentProcess,
                    PaymentPublishableToken = form.PaymentPublishableToken,
                    Version = form.Version,
                    CurrentVersion = form.CurrentVersion,
                    FolderId = form.FolderId,
                    Schema = string.IsNullOrEmpty(form.Schema) ? "{\"type\": \"form\", \"display\": \"form\"}" : form.Schema,
                    Script = form.Script,
                    RulesScript = form.RulesScript,
                    OriginalId = form.OriginalId,
                    IsIndex = "1",
                    IsEnabled = form.IsEnabled,
                    LockOnBuild = form.LockOnBuild,
                    LockToTenant = form.LockToTenant,
                    RequireAuth = form.RequireAuth
                };

                var output = new GetFormForEditOutput
                {
                    Form = Form,
                    FolderName = Form.FolderId != null ? (await _folderRepository.FirstOrDefaultAsync((Guid)Form.FolderId)).Name : ""
                };

                //
                output.Form.UserACLPermission = _ACLManager.FetchRole(new ACLCheckDto() { EntityId = (Guid)input.OriginalId, UserId = AbpSession.UserId, OrgId = null });// take the permission of the parent (max permission)

                return output;
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Forms)]
        public async Task<GetFormForEditOutput> GetFormForAuthor(GetFormForViewDto input)
        {

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            Form form = null;
            int.TryParse(input.Version, out int VersionInt);
            if (VersionInt != 0 && VersionInt.GetType() == typeof(int))
            {
                form = await GetFormByVersion((Guid)input.OriginalId, VersionInt);
            }
            else if (!string.IsNullOrEmpty(input.Version) && input.Version.GetType() == typeof(string))
            {
                form = await GetFormByVersion((Guid)input.OriginalId, null);
            }
            else
            {
                try
                {
                    form = await GetFormByVersion((Guid)input.OriginalId, null);
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException("Fetching Form failed because: " + ex);
                }
            }

            var Form = new CreateOrEditFormDto
            {
                Id = form.Id,
                Name = form.Name,
                Schema = string.IsNullOrEmpty(form.Schema) ? "{\"type\": \"form\", \"display\": \"form\"}" : form.Schema,
                Script = form.Script,
                RulesScript = form.RulesScript
            };

            var output = new GetFormForEditOutput
            {
                Form = Form
            };

            return output;

        }

        [AbpAuthorize(AppPermissions.Pages_Forms_Create, AppPermissions.Pages_Forms_Edit)]
        public async Task CreateOrEdit(CreateOrEditFormDto input)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
            input.VersionName = string.IsNullOrEmpty(input.VersionName) ? "Version 1" : input.VersionName;
            ACL aCL = new ACL() { UserId = AbpSession.UserId };
            var form = _formRepository.FirstOrDefaultAsync(i => i.Id == input.Id);
            if (form.Result == null)
            {
                aCL.Role = "O";
                await Create(aCL, input);
            }
            else
            {
                if (_ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = input.Id ?? input.OriginalId, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId }).IsAuthed)
                {
                    await Update(input);
                }
                else
                {
                    throw new UserFriendlyException("Not Authorised");
                }
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Forms_Create)]
        private async Task Create(ACL ACL, CreateOrEditFormDto input)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
            if (input.Id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                input.Id = Guid.NewGuid();
                input.OriginalId = (Guid)input.Id;
            }

            if (string.IsNullOrEmpty(input.Version.ToString()))
            {
                input.Version = 1;
            }

            if (string.IsNullOrEmpty(input.CurrentVersion.ToString()))
            {
                input.CurrentVersion = 1;
            }

            var form = ObjectMapper.Map<Form>(input);
            form.FormTypeId = (int)FormType.UserForm;
            if (AbpSession.TenantId != null)
            {
                form.TenantId = (int?)AbpSession.TenantId;
                ACL.TenantId = AbpSession.TenantId;
            }

            form.CreatorUserId = AbpSession.UserId;

            await _formRepository.InsertAsync(form);
            ACL.EntityID = form.Id;
            ACL.Type = "Form";
            await _ACLManager.AddACL(ACL);


            var jDiff = GetDataDiff(JToken.FromObject(form).ToString(), "{}");
            //when the created a form, update the version history	
            if (form.Version == form.CurrentVersion)
            {
                var rma = new EntityVersionHistory()
                {
                    Data = jDiff == null ? "{}" : jDiff.ToString(),
                    Name = form.Name,
                    Description = "Create a New Form", // 	
                    Version = form.Version,
                    VersionName = form.VersionName,
                    PreviousVersion = form.CurrentVersion, //live version	
                    EntityId = form.Id,
                    UserId = AbpSession.UserId,
                    TenantId = AbpSession.TenantId,
                    Type = "Form",
                    PreviousData = "{}",
                    NewData = JToken.FromObject(form).ToString(),
                };
                await _entityVersionHistoryRepository.InsertAsync(rma);

                ACL aCL = new ACL()
                {
                    UserId = AbpSession.UserId,
                    TenantId = AbpSession.TenantId,
                    EntityID = rma.Id,
                    Type = "EntityVersionHistory",
                    Role = "O",
                };
                await _ACLManager.AddACL(aCL);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Forms_Edit)]
        private async Task Update(CreateOrEditFormDto input)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
            var form = await _formRepository.FirstOrDefaultAsync((Guid)input.Id);

            //not set alive in the form builder, just change live version in the create and edit modal
            if (form.CurrentVersion == form.Version)
            {
                input.CurrentVersion = form.CurrentVersion;
            }

            var dataA = input.Schema; //new
            var dataB = form.Schema; //previous
            var jDiff = GetDataDiff(dataA, form.Schema);

            if (input.IsIndex == "1")
            {
                form.Name = input.Name;
                form.Description = input.Description;
                form.VersionName = input.VersionName;
                form.LockOnBuild = input.LockOnBuild;
                form.LockToTenant = input.LockToTenant;
                form.RequireAuth = input.RequireAuth;

                input.CreatorUserId = form.CreatorUserId;
            }
            else if (input.IsIndex == "2")
            {
                form.Name = input.Name;
                form.Description = input.Description;
                form.VersionName = input.VersionName;
                form.LockOnBuild = input.LockOnBuild;
                form.LockToTenant = input.LockToTenant;
                form.RequireAuth = input.RequireAuth;

                form.Schema = input.Schema;
                form.PaymentEnabled = input.PaymentEnabled;
                form.PaymentAmount = Convert.ToDecimal(input.PaymentAmount);
                form.PaymentCurrency = input.PaymentCurrency;
                form.PaymentProcess = input.PaymentProcess;
                form.PaymentProvider = "Stripe";

                input.CreatorUserId = form.CreatorUserId;
            }
            else if (input.IsIndex == "3")
            {
                input.Script = form.Script;
                input.RulesScript = form.RulesScript;
                input.LockOnBuild = form.LockOnBuild;
                input.LockToTenant = form.LockToTenant;
                input.RequireAuth = form.RequireAuth;

                input.PaymentEnabled = form.PaymentEnabled;
                input.PaymentAmount = Convert.ToDecimal(form.PaymentAmount);
                input.PaymentCurrency = form.PaymentCurrency;
                input.PaymentProcess = form.PaymentProcess;
                input.IsEnabled = form.IsEnabled;//

                input.CreatorUserId = form.CreatorUserId;

                //form.PaymentProvider = "Stripe";
                ObjectMapper.Map(input, form);
            }
            await _formRepository.UpdateAsync(form);


            //when the updated form is live version, and modify schema	
            if (form.Version == form.CurrentVersion && dataA != null && jDiff != null)
            {
                var rma = new EntityVersionHistory()
                {
                    Data = jDiff == null ? "{}" : jDiff.ToString(),
                    Name = form.Name,
                    Description = "Update the live Version", // 	
                    Version = form.Version,
                    VersionName = form.VersionName,
                    PreviousVersion = form.CurrentVersion, //live version	
                    EntityId = form.Id,
                    UserId = AbpSession.UserId,
                    TenantId = AbpSession.TenantId,
                    Type = "Form",
                    PreviousData = dataB,
                    NewData = dataA,
                };
                await _entityVersionHistoryRepository.InsertAsync(rma);

                ACL aCL = new ACL()
                {
                    UserId = AbpSession.UserId,
                    TenantId = AbpSession.TenantId,
                    EntityID = rma.Id,
                    Type = "EntityVersionHistory",
                    Role = "O",
                };
                await _ACLManager.AddACL(aCL);

            }



        }

        public async Task<int> CreateFormVersion(CreateVersionDto input)
        {

            if (_ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = input.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId }).IsAuthed ||
                _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = input.OriginalId, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId }).IsAuthed)
            {

                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
                Form newForm = new Form();
                newForm = ObjectMapper.Map<Form>(await _formRepository.FirstOrDefaultAsync(input.Id));
                newForm.Id = Guid.NewGuid();
                newForm.CreationTime = new DateTime();
                newForm.CreatorUserId = AbpSession.UserId;
                newForm.LastModificationTime = null;
                newForm.LastModifierUserId = null;

                int verno = _formRepository.GetAll().Where(n => n.OriginalId == input.OriginalId).Max(i => i.Version);

                newForm.Version = verno + 1;
                newForm.VersionName = input.VersionName;
                await _formRepository.InsertAsync(newForm);
                _formRuleRepository.GetAll().Where(i => i.FormId == input.Id).ToList().ForEach(i =>
                {
                    FormRule newFormRule = ObjectMapper.Map<FormRule>(i);
                    newFormRule.Id = Guid.NewGuid();
                    newFormRule.CreationTime = new DateTime();
                    newFormRule.CreatorUserId = AbpSession.UserId;
                    newFormRule.FormId = newForm.Id;
                    newForm.LastModificationTime = null;
                    newForm.LastModifierUserId = null;
                    _formRuleRepository.Insert(newFormRule);
                    newFormRule = new FormRule();
                });
                _appJobRepository.GetAll().Where(i => i.EntityId == input.Id).ToList().ForEach(i =>
                {
                    AppJob newAppJob = ObjectMapper.Map<AppJob>(i);
                    newAppJob.Id = Guid.NewGuid();
                    newAppJob.EntityId = newForm.Id;
                    _appJobRepository.Insert(newAppJob);
                    newAppJob = new AppJob();
                });
                return newForm.Version;
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Forms_Delete)]
        public async Task<MessageOutput> DeleteIndividual(EntityDto<Guid> input)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Delete", EntityId = input.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {

                await _formRepository.DeleteAsync(input.Id);

                return new MessageOutput()
                {
                    Message = "Template Removed",
                    Success = true
                };
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }


        }

        [AbpAuthorize(AppPermissions.Pages_Templates_Delete)]
        public async Task<MessageOutput> DeleteAll(EntityDto<Guid> input)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = input.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    var AllFormsByOriginalId = _formRepository.GetAll().Where(i => i.OriginalId == input.Id);
                    await AllFormsByOriginalId.ForEachAsync(async i =>
                    {
                        await _formRepository.DeleteAsync(i.Id);
                    });
                    unitOfWork.Complete();
                }
                return new MessageOutput()
                {
                    Message = "Templates Removed",
                    Success = true
                };
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }


        public async Task<GetFormForLoad> GetFormLoadProjectView(GetFormForViewDto input, Guid? ProjectId)// (EntityDto<Guid> input)
        {

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
            string _tenantName = "Host";
            Form form = null;
            int.TryParse(input.Version, out int VersionInt);
            if (VersionInt != 0 && VersionInt.GetType() == typeof(int))
            {
                form = await GetFormByVersion((Guid)input.OriginalId, VersionInt);
            }
            else if (!string.IsNullOrEmpty(input.Version) && input.Version.GetType() == typeof(string))
            {
                form = await GetFormByVersion((Guid)input.OriginalId, null);
            }
            else
            {
                try
                {
                    form = await GetFormByVersion((Guid)input.OriginalId, null);
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException("Fetching Forms failed because: " + ex);
                }
            }
            if (AbpSession.TenantId != null)
            {
                _tenantName = await TenantManager.GetTenantName((int)AbpSession.TenantId);
            }

            /*			Hide Clear/Save/Submit buttons when form loaded in Project menu
						Render those buttons in other views*/
            form.Schema = DisplayHideFormButtons(form.Schema, true);

            GetFormForLoad result = new GetFormForLoad()
            {
                AuthToken = JwtSecurityTokenProvider.GenerateToken(DateTime.Now.ToString(), _jwtExpiry),
                AnonAuthToken = JwtSecurityTokenProvider.GenerateToken(DateTime.Now.ToString(), _jwtExpiry),
                TenantName = _tenantName,
                FormId = form.Id,
                FormName = form.Name,
                FormData = null,
                RecordId = null,
                RecordMatterId = null,
                SubmissionId = form.Id,
                TenantId = form.TenantId
            };
            return result;
        }

        public async Task<GetFormForLoad> GetFormFromReleaseView(Guid projectId, Guid formId)
        {

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            Project p = _projectRepository.FirstOrDefault((Guid)projectId);

            GetFormFromReleaseOutput rOutput = _projectManager.GetFormFromRelease((Guid)p.ReleaseId, (Guid)formId);
            if (rOutput.Form != null)
            {
                Form f = rOutput.Form;

                string _tenantName = "Host";
                if (AbpSession.TenantId != null)
                {
                    _tenantName = await TenantManager.GetTenantName((int)AbpSession.TenantId);
                }

                // *Hide Clear/Save/Submit buttons when form loaded in Project menu Render those buttons in other views*/
                f.Schema = DisplayHideFormButtons(f.Schema, true);

                GetFormForLoad result = new GetFormForLoad()
                {
                    AuthToken = JwtSecurityTokenProvider.GenerateToken(DateTime.Now.ToString(), _jwtExpiry),
                    AnonAuthToken = JwtSecurityTokenProvider.GenerateToken(DateTime.Now.ToString(), _jwtExpiry),
                    TenantName = _tenantName,
                    FormId = f.Id,
                    FormName = f.Name,
                    FormData = null,
                    RecordId = null,
                    RecordMatterId = null,
                    ReleaseId = p.ReleaseId,
                    SubmissionId = f.Id,
                    TenantId = f.TenantId,
                    Schema = f.Schema
                };

                return result;

            }
            else
            {
                throw new UserFriendlyException("Project Release Not Found");
            }

        }

        public async Task<GetFormForLoad> GetFormLoadView(GetFormForViewDto input)// (EntityDto<Guid> input)
        {

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
            string _tenantName = "Host";
            Form form = null;
            int.TryParse(input.Version, out int VersionInt);
            if (VersionInt != 0 && VersionInt.GetType() == typeof(int))
            {
                form = await GetFormByVersion((Guid)input.OriginalId, VersionInt);
            }
            else if (!string.IsNullOrEmpty(input.Version) && input.Version.GetType() == typeof(string))
            {
                form = await GetFormByVersion((Guid)input.OriginalId, null);
            }
            else
            {
                try
                {
                    form = await GetFormByVersion((Guid)input.OriginalId, null);
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException("Fetching Forms failed because: " + ex);
                }
            }
            if (AbpSession.TenantId != null)
            {
                _tenantName = await TenantManager.GetTenantName((int)AbpSession.TenantId);
            }

            GetFormForLoad result = new GetFormForLoad()
            {
                AuthToken = JwtSecurityTokenProvider.GenerateToken(DateTime.Now.ToString(), _jwtExpiry),
                AnonAuthToken = JwtSecurityTokenProvider.GenerateToken(DateTime.Now.ToString(), _jwtExpiry),
                TenantName = _tenantName,
                FormId = form.Id,
                FormName = form.Name,
                FormData = null,
                RecordId = null,
                RecordMatterId = null,
                SubmissionId = form.Id
            };

            if (input.RecordMatterId.HasValue && input.RecordMatterId != Guid.Empty)
            {
                var recordId = (await _recordMatterRepository.GetAsync((Guid)input.RecordMatterId)).RecordId;
                result.ProjectName = (await _projectRepository.FirstOrDefaultAsync(e => e.RecordId == recordId))?.Name;
            }

            return result;
        }

        private string DisplayHideFormButtons(string schema, bool isProject)
        {
            string sbuttons;
            if (isProject)
            {
                sbuttons = @"{
					""save_label"": ""Save"",
					""save_disable"": ""true"",
					""save_hide"": ""true"",
					""submit_label"": ""Submit"",
					""submit_disable"": ""true"",
					""submit_hide"": ""true"",
					""clear_label"": ""Clear"",
					""clear_disable"": ""true"",
					""clear_hide"": ""true"",
					""next_label"": ""Next"",
					""previous_label"": ""Previous""
				}";
            }
            else
            {
                sbuttons = @"{
					""save_label"": ""Save"",
					""save_disable"": ""false"",
					""save_hide"": ""false"",
					""submit_label"": ""Submit"",
					""submit_disable"": ""false"",
					""submit_hide"": ""false"",
					""clear_label"": ""Clear"",
					""clear_disable"": ""false"",
					""clear_hide"": ""false"",
					""next_label"": ""Next"",
					""previous_label"": ""Previous""
				}";
            }

            JObject jBtns = JObject.Parse(sbuttons);
            JObject jForms = JObject.Parse(schema);
            jForms["buttons"] = jBtns;
            schema = jForms.ToString();

            return schema;
        }

        [AllowAnonymous]
        public async Task<GetFormForView> GetFormForView(GetFormForViewDto input)
        {

            if (!input.OriginalId.HasValue && string.IsNullOrEmpty(input.AccessToken)) return null;

            GetFormForView result = new GetFormForView();

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            if (_recordMatterContributorRepository.GetAll().Any(e => e.AccessToken == input.AccessToken))
            {
                result = await GetFormForContributorView(input);
                result.Form.Schema = DisplayHideFormButtons(result.Form.Schema, true);
            }
            else
            {
                // Is the User Opening this form as an Author for a Project
                var recordmatter = _recordMatterRepository.GetAll().FirstOrDefault(e => e.Id == input.RecordMatterId);
                if (recordmatter != null)
                {
                    if (_projectRepository.GetAll().Any(e => e.RecordId == recordmatter.RecordId && e.CreatorUserId == AbpSession.UserId))
                    {
                        result = await GetFormForAuthorView(input, recordmatter);
                        result.Form.Schema = DisplayHideFormButtons(result.Form.Schema, true);

                    }
                    else
                    {
                        result = await GetFormForUserView(input);
                    }
                }
                else
                {
                    result = await GetFormForUserView(input);
                }

                if (result.Contributor != null)
                {
                    result.Contributor.Status = recordmatter.Status == null ? RecordMatterConsts.RecordMatterStatus.New : recordmatter.Status;
                }
            }

            //Add project Details
            var rm = _recordMatterRepository.FirstOrDefault(e => e.Id == input.RecordMatterId);
            if (rm != null)
            {
                var p = _projectRepository.FirstOrDefault(e => e.RecordId == rm.RecordId);
                if (p != null)
                {
                    result.Project = ObjectMapper.Map<ProjectDto>(p);
                }

            }

            //
            result.UserACLPermission = _ACLManager.FetchRole(new ACLCheckDto() { EntityId = (Guid)input.OriginalId, UserId = AbpSession.UserId, OrgId = null });// take the permission of the parent (max permission)

            return result;
        }

        private async Task<GetFormForView> GetFormForAuthorView(GetFormForViewDto input, RecordMatter recordmatter)// (EntityDto<Guid> input)
        {
            GetFormForView result = new GetFormForView();

            result = await GetFormForUserView(input);

            result.Contributor = new GetRecordMatterContributorForViewDto
            {
                RecordMatterContributor = new RecordMatterContributorDto()
                {
                    StepRole = ProjectConsts.ProjectStepRole.Author
                },
                Message = GetFormForViewMessage((Guid)recordmatter.Id)
            };


            if (recordmatter.Status == RecordMatterConsts.RecordMatterStatus.Share)
            {
                var recordMatterContributor = await _recordMatterContributorRepository.GetAll()
                                            .Where(e => e.RecordMatterId == recordmatter.Id).ToListAsync();

                foreach (var r in recordMatterContributor)
                {
                    if (r.Status == 0 && r.Enabled == true)
                    {
                        result.Form.Schema = GetFormForViewButtons(result.Form.Schema, (Guid)input.RecordMatterId);
                        result.Form.ReadOnly = true;
                        break;
                    }
                }
            }
            //add antother logic getformForContributor: if is waiting
            //when form in publish status, record contrubutor in the waiting status
            if (recordmatter.Status == RecordMatterConsts.RecordMatterStatus.Final)
            {
                result.Form.Schema = GetFormForViewButtons(result.Form.Schema, (Guid)input.RecordMatterId);
                result.Form.ReadOnly = true;
            }

            result.Contributor.RequireApproval = recordmatter.RequireApproval;
            result.Contributor.RequireReview = recordmatter.RequireReview;

            return result;

        }

        private async Task<GetFormForView> GetFormForContributorView(GetFormForViewDto input)// (EntityDto<Guid> input)
        {

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            GetFormForView result = new GetFormForView();
            result.ReleaseId = input.ReleaseId;

            Form form = null;

            var recordMatterContributor = _recordMatterContributorRepository.FirstOrDefault(e => e.AccessToken == input.AccessToken);

            if (recordMatterContributor != null && recordMatterContributor.Enabled)
            {

                ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
                {
                    Action = "Edit",
                    EntityId = (Guid)recordMatterContributor.RecordMatterId,
                    UserId = AbpSession.UserId,
                    AccessToken = input.AccessToken,
                    TenantId = AbpSession.TenantId
                });

                if (ACLResult.IsAuthed)
                {

                    form = _formRepository.FirstOrDefault((Guid)recordMatterContributor.FormId);
                    JObject jform = JObject.Parse(recordMatterContributor.FormSchema);

                    result.Form = new FormDto();
                    result.Form.Schema = jform.ToString();

                    result.Form.Id = (Guid)recordMatterContributor.FormId;
                    result.Form.Name = form.Name;
                    result.Contributor = new GetRecordMatterContributorForViewDto { RecordMatterContributor = ObjectMapper.Map<RecordMatterContributorDto>(recordMatterContributor) };
                    result.Contributor.Message = GetFormForViewMessage((Guid)recordMatterContributor.RecordMatterId);

                    result.Form.RulesScript = form.RulesScript;

                    //recordMatterContributor.RecordMatterFk.Status == RecordMatterConsts.RecordMatterStatus.Publish ||  publish should allow contributor to edit
                    if ((recordMatterContributor.RecordMatterFk.Status == RecordMatterConsts.RecordMatterStatus.Share && recordMatterContributor.StepRole == ProjectConsts.ProjectStepRole.Approve) || recordMatterContributor.RecordMatterFk.Status == RecordMatterConsts.RecordMatterStatus.Final)
                        result.Form.ReadOnly = true;

                    // Add documents if approver and documents exist
                    if (recordMatterContributor.StepRole == Projects.ProjectConsts.ProjectStepRole.Approve)
                    {
                        result.Documents = new List<GetDocumentForView>();

                        var documents = _recordMatterItemRepository.GetAll().Where(e => e.RecordMatterId == recordMatterContributor.RecordMatterId);

                        documents.ForEach(e =>
                        {

                            if (e.Document != null)
                            {
                                if (e.AllowedFormats.ToLower().Contains("p"))
                                {
                                    result.Documents.Add(new GetDocumentForView()
                                    {
                                        Id = e.Id,
                                        Name = e.DocumentName,
                                        Format = "pdf"
                                    });
                                }
                                if (e.AllowedFormats.ToLower().Contains("w"))
                                {
                                    result.Documents.Add(new GetDocumentForView()
                                    {
                                        Id = e.Id,
                                        Name = e.DocumentName,
                                        Format = "docx"
                                    });
                                }
                            }
                        });
                        if (documents.Count() != 0)
                        {
                            result.hasDocuments = true;
                        }
                    }

                }
                else
                {

                    result.Form = new FormDto();
                    result.Form.Name = "No_Access_For_Syntaq";
                    result.Form.Schema = @"{
						""type"": ""form"",
						""display"": ""form"",
						""components"": [
						{
								""key"": ""label"",
								""labelvalue"": ""Your permission to access this project has expired."",
								""tooltip"": """",

                                ""bold"": false,

								""widthslider"": ""12"",
								""offsetslider"": ""0"",
								""tabindex"": """",
								""type"": ""label"",
								""label"": ""label"",
								""tableView"": false,
								""input"": false
						},
						{
								""key"": ""label"",
								""labelvalue"": ""You were granted access to the project for a period of 20 days, and this duration has now passed. Please contact the project author to regain access to the project."",
								""tooltip"": """",

                                ""bold"": false,
								""widthslider"": ""12"",
								""offsetslider"": ""0"",
								""tabindex"": """",
								""type"": ""label"",
								""label"": ""label"",
								""tableView"": false,
								""input"": false
							}
						],
						""buttons"": {
							""save_label"": ""Save"",
							""save_disable"": ""true"",
							""save_hide"": ""true"",
							""submit_label"": ""Submit"",
							""submit_disable"": ""true"",
							""submit_hide"": ""true"",
							""clear_label"": ""Clear"",
							""clear_disable"": ""true"",
							""clear_hide"": ""true"",
							""next_label"": ""Next"",
							""previous_label"": ""Previous""
						}	
					}";

                    return result;

                }                

            }
            else
            {
                result.Form = new FormDto();
                result.Form.Name = "No_Access_For_Syntaq";
                result.Form.Schema = @"{
						""type"": ""form"",
						""display"": ""form"",
						""components"": [
						{
								""key"": ""label"",
								""labelvalue"": ""Thank you for your contribution."",
								""tooltip"": """",

                                ""bold"": false,

								""widthslider"": ""12"",
								""offsetslider"": ""0"",
								""tabindex"": """",
								""type"": ""label"",
								""label"": ""label"",
								""tableView"": false,
								""input"": false
						},
						{
								""key"": ""label"",
								""labelvalue"": ""If you require further access to this project, please contact the Author."",
								""tooltip"": """",

                                ""bold"": false,
								""widthslider"": ""12"",
								""offsetslider"": ""0"",
								""tabindex"": """",
								""type"": ""label"",
								""label"": ""label"",
								""tableView"": false,
								""input"": false
							}
						],
						""buttons"": {
							""save_label"": ""Save"",
							""save_disable"": ""true"",
							""save_hide"": ""true"",
							""submit_label"": ""Submit"",
							""submit_disable"": ""true"",
							""submit_hide"": ""true"",
							""clear_label"": ""Clear"",
							""clear_disable"": ""true"",
							""clear_hide"": ""true"",
							""next_label"": ""Next"",
							""previous_label"": ""Previous""
						}	
					}
				";

                if (AbpSession.UserId != null)
                {
                    throw new UserFriendlyException("Your access to this project has been restricted by the Project Author. If you require access please contact the Author.");
                }
                else
                {
                    return result;
                }


            }

            result = await BuildNestedForms(result);
            result.Form.Schema = GetFormForViewButtons(result.Form.Schema, (Guid)input.RecordMatterId);

            return result;

        }

        private string GetFormForViewButtons(string schema, Guid recordmatterid)
        {
            string result = schema;

            // Set button based on status for Step
            string sbuttons = @"{
							""save_label"": ""Save"",
							""save_disable"": ""false"",
							""save_hide"": ""false"",
							""submit_label"": ""Submit"",
							""submit_disable"": ""true"",
							""submit_hide"": ""true"",
							""clear_label"": ""Clear"",
							""clear_disable"": ""true"",
							""clear_hide"": ""true"",
							""next_label"": ""Next"",
							""previous_label"": ""Previous""
						}";

            var recordmatter = _recordMatterRepository.GetAll().FirstOrDefault(e => e.Id == recordmatterid);
            if (recordmatter != null)
            {
                if (recordmatter.Status == RecordMatterConsts.RecordMatterStatus.Final || recordmatter.Status == RecordMatterConsts.RecordMatterStatus.Share)
                {
                    sbuttons = @"{
									""save_label"": ""Save"",
									""save_disable"": ""true"",
									""save_hide"": ""true"",
									""submit_label"": ""Submit"",
									""submit_disable"": ""true"",
									""submit_hide"": ""true"",
									""clear_label"": ""Clear"",
									""clear_disable"": ""true"",
									""clear_hide"": ""true"",
									""next_label"": ""Next"",
									""previous_label"": ""Previous""
								}";
                }
            }

            JObject jBtns = JObject.Parse(sbuttons);
            JObject jForms = JObject.Parse(result);
            jForms["buttons"] = jBtns;
            result = jForms.ToString();

            return result;

        }

        private string GetFormForViewMessage(Guid recordmatterid)
        {
            string result = string.Empty;

            var recordmatter = _recordMatterRepository.GetAll().FirstOrDefault(e => e.Id == recordmatterid);
            if (recordmatter != null)
            {
                result = recordmatter.Status.ToString();
                if (recordmatter.Status == RecordMatterConsts.RecordMatterStatus.Share)
                {

                    var recordMatterContributor = _recordMatterContributorRepository.GetAll()
                                            .Where(e => e.RecordMatterId == recordmatter.Id).OrderByDescending(c => c.CreationTime).FirstOrDefault();

                    if (recordMatterContributor != null)
                    {
                        result = $"{Convert.ToString(recordMatterContributor.Status)} {Convert.ToString(recordMatterContributor.StepRole)}";
                    }

                    //foreach (var recordMatterCon in recordMatterContributor)
                    //{

                    //	result = $"{Convert.ToString(recordMatterCon.Status)} {Convert.ToString(recordMatterCon.StepRole)}";
                    // result = $"{Convert.ToString(recordMatterCon.Status)}";

                    //  Enum should hev been updated - do add an "A" this is weakly typed and easily broken and unesscesry
                    //if (recordMatterCon.Status == 0 & recordMatterCon.Enabled == true)
                    //{
                    //	//showing awaiting
                    //	result = "A" + recordMatterCon.Status.ToString().ToLower() + " " + recordMatterCon.StepRole.ToString();
                    //	break;

                    //}
                    //result = recordMatterCon.Status.ToString() + " " + recordMatterCon.StepRole.ToString();

                    //}

                }
            }

            return result;

        }

        private async Task<GetFormForView> GetFormForUserView(GetFormForViewDto input)// (EntityDto<Guid> input)
        {

            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            GetFormForView result = new GetFormForView();
            Form form = null;

            if (input.ReleaseId.HasValue)
            {
                // Get From Release Package
                GetFormFromReleaseOutput rOutput = _projectManager.GetFormFromRelease((Guid)input.ReleaseId, (Guid)input.Id);
                form = rOutput.Form;

                if (rOutput.AppJobs.Count > 0)
                {
                    CreateOrEditAppJobDto jobClass = JsonConvert.DeserializeObject<CreateOrEditAppJobDto>(rOutput.AppJobs[0].Data);
                    if (jobClass.Document.Count > 0 && jobClass.DeleteRecordsAfterAssembly == false)
                        result.hasDocuments = true;
                }

            }
            else
            {

                if (input.Id != null)
                {
                    form = _formRepository.FirstOrDefault((Guid)input.Id);
                }

                if (!input.OriginalId.HasValue)
                {
                    input.OriginalId = form.OriginalId;
                }


                int.TryParse(input.Version, out int VersionInt);
                if (VersionInt != 0 && VersionInt.GetType() == typeof(int))
                {
                    form = await GetFormByVersion((Guid)input.OriginalId, VersionInt);
                }
                else if (!string.IsNullOrEmpty(input.Version) && input.Version.GetType() == typeof(string))
                {
                    form = await GetFormByVersion((Guid)input.OriginalId, null);
                }
                else
                {
                    try
                    {
                        form = _formRepository.FirstOrDefault((Guid)input.Id);
                    }
                    catch (Exception ex)
                    {
                        throw new UserFriendlyException("Fetching Forms failed because: " + ex);
                    }
                }

                // Check if this form is locked to tenancy
                if (form.LockToTenant)
                {
                    if (AbpSession.TenantId != form.TenantId)
                    {
                        throw new UserFriendlyException("Permission Denied");
                    }
                }

            }


            result = new GetFormForView { Form = ObjectMapper.Map<FormDto>(form) };
            result.AnonAuthToken = JwtSecurityTokenProvider.GenerateToken(DateTime.Now.ToString(), _jwtExpiry);

            var appjob = _appJobRepository.GetAll().FirstOrDefault(i => i.EntityId == input.Id);
            if (appjob != null)
            {
                CreateOrEditAppJobDto jobClass = JsonConvert.DeserializeObject<CreateOrEditAppJobDto>(appjob.Data);
                if (jobClass.Document.Count > 0 && jobClass.DeleteRecordsAfterAssembly == false)
                {
                    result.hasDocuments = true;
                }
            }

            // Check to see if the recordmatter has already been built
            var LockOnBuild = false;
            if (input.RecordMatterItemId.HasValue)
            {
                var rmi = _recordMatterItemRepository.FirstOrDefault(i => i.Id == (Guid)input.RecordMatterItemId);
                if (rmi != null)
                {
                    LockOnBuild = rmi.LockOnBuild;
                }
            }

            if (form.RequireAuth && AbpSession.UserId == null)
            {
                // check the record matter to determine if allows re-submit
                result.Form.Schema = "{\"type\":\"form\",\"display\":\"form\",\"components\":[{\"title\":\"Authentication\",\"key\":\"notesHelpEditor\",\"htmlcontent\":\"This Form requires an authenticated user.\",\"widthslider\":\"12\",\"offsetslider\":\"0\",\"type\":\"helpnotes\",\"input\":false,\"tableView\":false,\"label\":\"Notes /Help editor\"}]}";
                result.Form.Script = "$(\".btn-form-nav-Clear\").prop(\"hidden\", true);";
                result.Form.Script += "$(\".btn-form-nav-Save\").prop(\"hidden\", true);";
                result.Form.Script += "$(\".btn-form-nav-Submit\").prop(\"hidden\", true);";
                return result;
            }

            if (form.LockToTenant && (form.TenantId != AbpSession.TenantId || AbpSession.UserId == null))
            {

                var formids = _ACLManager.GetAllSharedEntities(AbpSession.TenantId, "Form");
                if (!formids.Contains(form.Id))
                {
                    // check the record matter to determine if allows re-submit
                    result.Form.Schema = "{\"type\":\"form\",\"display\":\"form\",\"components\":[{\"title\":\"Form Locked\",\"key\":\"notesHelpEditor\",\"htmlcontent\":\"This Form is not enabled for this Tenant.\",\"widthslider\":\"12\",\"offsetslider\":\"0\",\"type\":\"helpnotes\",\"input\":false,\"tableView\":false,\"label\":\"Notes /Help editor\"}]}";
                    result.Form.Script = "$(\".btn-form-nav-Clear\").prop(\"hidden\", true);";
                    result.Form.Script += "$(\".btn-form-nav-Save\").prop(\"hidden\", true);";
                    result.Form.Script += "$(\".btn-form-nav-Submit\").prop(\"hidden\", true);";
                    return result;
                }


            }

            if (LockOnBuild)
            {
                // check the record matter to determine if allows re-submit
                result.Form.Schema = "{\"type\":\"form\",\"display\":\"form\",\"components\":[{\"title\":\"Record is Locked\",\"key\":\"notesHelpEditor\",\"htmlcontent\":\"This Record Matter Item is locked once it has been built.\",\"widthslider\":\"12\",\"offsetslider\":\"0\",\"type\":\"helpnotes\",\"input\":false,\"tableView\":false,\"label\":\"Notes /Help editor\"}]}";
                result.Form.Script = "$(\".btn-form-nav-Clear\").prop(\"hidden\", true);";
                result.Form.Script += "$(\".btn-form-nav-Save\").prop(\"hidden\", true);";
                result.Form.Script += "$(\".btn-form-nav-Submit\").prop(\"hidden\", true);";
                return result;
            }

            if (string.IsNullOrEmpty(form.Schema) || form.IsEnabled == false)
            {
                result.Form.Schema = "{\"type\":\"form\",\"display\":\"form\",\"components\":[{\"title\":\"Disabled Form\",\"key\":\"notesHelpEditor\",\"htmlcontent\":\"This Form is currently disabled. Please contact your system administrator.\",\"widthslider\":\"12\",\"offsetslider\":\"0\",\"type\":\"helpnotes\",\"input\":false,\"tableView\":false,\"label\":\"Notes /Help editor\"}]}";
                result.Form.Script = "$(\".btn-form-nav-Clear\").prop(\"hidden\", true);";
                result.Form.Script += "$(\".btn-form-nav-Save\").prop(\"hidden\", true);";
                result.Form.Script += "$(\".btn-form-nav-Submit\").prop(\"hidden\", true);";
                return result;
            }
            else
            {
                JObject SchemaJObject = JObject.Parse(result.Form.Schema);
                bool HasNested = SchemaJObject.HasMatchingToken("type", "nestedform");
                bool PaymentRequired = result.Form.PaymentEnabled;
                bool IsWizard = SchemaJObject.HasMatchingToken("display", "wizard");
                dynamic formSchema = new ExpandoObject();
                string RulesCollection = null;
                string ScriptCollection = null;
                List<dynamic> FormComponents = new List<dynamic>();

                if ((PaymentRequired || HasNested))
                {
                    formSchema.type = "form";
                    formSchema.display = PaymentRequired || IsWizard ? "wizard" : "form";
                    formSchema.components = new List<dynamic>();
                    if (PaymentRequired && result.Form.PaymentProcess == "BFL")
                    {
                        PageSettingsClass pageSettings = new PageSettingsClass()
                        {
                            Breadcrumb = "default",
                            BreadcrumbClickable = false,
                            Collapsed = false,
                            Collapsible = false,
                            Key = "Payment",
                            Label = "Payment",
                            IsWizard = true
                        };
                        dynamic BFLPaymentPage = CreateDynamicPage(pageSettings);
                        Form PaymentForm = await FetchPaymentForm(result.Form.Id);

                        if (PaymentForm != null)
                        {
                            RulesCollection += PaymentForm.RulesScript ?? string.Empty;

                            // Add the default first so that it can be over ridden
                            ScriptCollection = "$(Form).setVal(\"PaymentProcess\", \"BDA\"); $(Form).setVal(\"PaymentAmount\", \"" + result.Form.PaymentAmount + "\");" + ScriptCollection;
                            ScriptCollection += PaymentForm.Script;
                            List<FormIOSchemaComponentClass> PaymentComponents = await FetchComponentsListFromSchema(PaymentForm.Schema);
                            FormComponents = new List<dynamic>();


                            Guid releaseId = input.ReleaseId == null ? Guid.Empty : (Guid)input.ReleaseId;
                            await ParseFormIOComponents(releaseId, PaymentComponents, ref FormComponents, ref RulesCollection, ref ScriptCollection);
                            BFLPaymentPage.components = FormComponents;
                            formSchema.components.Add(BFLPaymentPage);
                        }

                    }

                    if (IsWizard)
                    {
                        RulesCollection += result.Form.RulesScript;
                        ScriptCollection += result.Form.Script;
                        List<FormIOSchemaComponentClass> WizardComponents = await FetchComponentsListFromSchema(result.Form.Schema);
                        WizardComponents.ForEach(async i =>
                        {
                            PageSettingsClass pageSettings = new PageSettingsClass()
                            {
                                Breadcrumb = i.Breadcrumb,
                                BreadcrumbClickable = i.BreadcrumbClickable ?? true,
                                Collapsed = i.Collapsed ?? false,
                                Collapsible = i.Collapsible ?? false,
                                Key = i.Key,
                                Label = i.Label,
                                IsWizard = IsWizard
                            };
                            FormComponents = new List<dynamic>();
                            dynamic WizardPage = CreateDynamicPage(pageSettings);
                            WizardPage.ShowSummary = i.ShowSummary ?? false;
                            WizardPage.HideLabel = i.HideLabel ?? false;
                            WizardPage.ApplyLabelFirstRow = i.ApplyLabelFirstRow ?? false;

                            Guid releaseId = input.ReleaseId == null ? Guid.Empty : (Guid)input.ReleaseId;

                            await ParseFormIOComponents(releaseId, i.Components, ref FormComponents, ref RulesCollection, ref ScriptCollection);
                            WizardPage.components = FormComponents;
                            formSchema.components.Add(WizardPage);
                        });
                        if (SchemaJObject.HasMatchingToken("isBuildTicks"))
                        {
                            formSchema.isBuildTicks = SchemaJObject.GetValue("isBuildTicks");
                        }
                    }
                    else
                    {
                        RulesCollection += form.RulesScript;
                        ScriptCollection += form.Script;
                        List<FormIOSchemaComponentClass> PageComponents = await FetchComponentsListFromSchema(result.Form.Schema);
                        FormComponents = new List<dynamic>();

                        Guid releaseid = input.ReleaseId == null ? Guid.Empty : (Guid)input.ReleaseId;
                        FormComponents = await ParseFormIOComponents(releaseid, await FetchComponentsListFromSchema(result.Form.Schema), ref FormComponents, ref RulesCollection, ref ScriptCollection);

                        if (PaymentRequired)
                        {
                            PageSettingsClass pageSettings = new PageSettingsClass()
                            {
                                Breadcrumb = "default",
                                BreadcrumbClickable = true,
                                Collapsed = false,
                                Collapsible = false,
                                Key = "Nested",
                                Label = form.Name,
                                IsWizard = IsWizard
                            };
                            dynamic NestedPage = CreateDynamicPage(pageSettings);
                            NestedPage.components = FormComponents;
                            formSchema.components.Add(NestedPage);
                        }
                        else
                        {
                            formSchema.components = FormComponents;
                        }
                    }

                    if (PaymentRequired && result.Form.PaymentProcess == "BDA")
                    {
                        PageSettingsClass pageSettings = new PageSettingsClass()
                        {
                            Breadcrumb = "default",
                            BreadcrumbClickable = false,
                            Collapsed = false,
                            Collapsible = false,
                            Key = "Payment",
                            Label = "Payment",
                            IsWizard = PaymentRequired
                        };

                        dynamic BFLPaymentPage = CreateDynamicPage(pageSettings);
                        Form PaymentForm = await FetchPaymentForm(result.Form.Id);

                        if (PaymentForm != null)
                        {
                            RulesCollection += PaymentForm.RulesScript;
                            // Add the default first so that it can be over ridden
                            ScriptCollection = "$(Form).setVal(\"PaymentProcess\", \"BDA\"); $(Form).setVal(\"PaymentAmount\", \"" + result.Form.PaymentAmount + "\");" + ScriptCollection;
                            ScriptCollection += PaymentForm.Script;

                            ScriptCollection += PaymentForm.Script;

                            List<FormIOSchemaComponentClass> PaymentComponents = await FetchComponentsListFromSchema(PaymentForm.Schema);
                            FormComponents = new List<dynamic>();
                            Guid releaseId = input.ReleaseId == null ? Guid.Empty : (Guid)input.ReleaseId;
                            await ParseFormIOComponents(releaseId, PaymentComponents, ref FormComponents, ref RulesCollection, ref ScriptCollection);
                            BFLPaymentPage.components = FormComponents;
                            formSchema.components.Add(BFLPaymentPage);
                        }

                    }
                }
                else
                {
                    return result;
                }

                SchemaButtonsClass frmBtns = CreateDynamicButtons(form, IsWizard);
                formSchema.buttons = frmBtns;

                if (SchemaJObject.HasMatchingToken("autoSaving"))
                {
                    formSchema.autoSaving = SchemaJObject.GetValue("autoSaving");
                }
                if (SchemaJObject.HasMatchingToken("feedbackForm"))
                {
                    formSchema.feedbackForm = SchemaJObject.GetValue("feedbackForm");
                }

                JsonSerializerSettings JsonSetting = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.None
                };

                result.Form.Schema = JsonConvert.SerializeObject(formSchema, JsonSetting);
                result.Form.RulesScript = RulesCollection;
                result.Form.Script = ScriptCollection;
            }

            return result;

        }

        private async Task<GetFormForView> BuildNestedForms(GetFormForView input)
        {
            GetFormForView result = input;

            JObject SchemaJObject = JObject.Parse(input.Form.Schema);

            dynamic formSchema = new ExpandoObject();
            string RulesCollection = null;
            string ScriptCollection = null;
            List<dynamic> FormComponents = new List<dynamic>();

            bool IsWizard = SchemaJObject.HasMatchingToken("display", "wizard");
            bool PaymentRequired = result.Form.PaymentEnabled;

            formSchema.type = "form";
            formSchema.components = new List<dynamic>();

            if (IsWizard)
            {
                formSchema.display = "wizard";

                RulesCollection += result.Form.RulesScript;
                ScriptCollection += result.Form.Script;
                List<FormIOSchemaComponentClass> WizardComponents = await FetchComponentsListFromSchema(result.Form.Schema);
                WizardComponents.ForEach(async i =>
                {
                    PageSettingsClass pageSettings = new PageSettingsClass()
                    {
                        Breadcrumb = i.Breadcrumb,
                        BreadcrumbClickable = i.BreadcrumbClickable ?? true,
                        Collapsed = i.Collapsed ?? false,
                        Collapsible = i.Collapsible ?? false,
                        Key = i.Key,
                        Label = i.Label,
                        IsWizard = IsWizard
                    };
                    FormComponents = new List<dynamic>();
                    dynamic WizardPage = CreateDynamicPage(pageSettings);
                    WizardPage.ShowSummary = i.ShowSummary ?? false;
                    WizardPage.HideLabel = i.HideLabel ?? false;
                    WizardPage.ApplyLabelFirstRow = i.ApplyLabelFirstRow ?? false;
                    Guid releaseId = input.ReleaseId == null ? Guid.Empty : (Guid)input.ReleaseId;
                    await ParseFormIOComponents(releaseId, i.Components, ref FormComponents, ref RulesCollection, ref ScriptCollection);
                    WizardPage.components = FormComponents;
                    formSchema.components.Add(WizardPage);
                });
            }
            else
            {
                formSchema.display = "form";

                RulesCollection += result.Form.RulesScript;
                ScriptCollection += result.Form.Script;
                List<FormIOSchemaComponentClass> PageComponents = await FetchComponentsListFromSchema(result.Form.Schema);
                FormComponents = new List<dynamic>();
                Guid releaseId = input.ReleaseId == null ? Guid.Empty : (Guid)input.ReleaseId;
                FormComponents = await ParseFormIOComponents(releaseId, await FetchComponentsListFromSchema(result.Form.Schema), ref FormComponents, ref RulesCollection, ref ScriptCollection);
                if (PaymentRequired)
                {
                    PageSettingsClass pageSettings = new PageSettingsClass()
                    {
                        Breadcrumb = "default",
                        BreadcrumbClickable = true,
                        Collapsed = false,
                        Collapsible = false,
                        Key = "Nested",
                        Label = result.Form.Name,
                        IsWizard = IsWizard
                    };
                    dynamic NestedPage = CreateDynamicPage(pageSettings);
                    NestedPage.components = FormComponents;
                    formSchema.components.Add(NestedPage);
                }
                else
                {
                    formSchema.components = FormComponents;
                }
            }

            if (SchemaJObject.HasMatchingToken("autoSaving"))
            {
                formSchema.autoSaving = SchemaJObject.GetValue("autoSaving");
            }
            if (SchemaJObject.HasMatchingToken("feedbackForm"))
            {
                formSchema.feedbackForm = SchemaJObject.GetValue("feedbackForm");
            }

            JsonSerializerSettings JsonSetting = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.None
            };

            result.Form.Schema = JsonConvert.SerializeObject(formSchema, JsonSetting);
            result.Form.RulesScript = RulesCollection;
            result.Form.Script = ScriptCollection;

            return result;
        }

        private async Task<Form> FetchPaymentForm(Guid FormId)
        {
            Form result = null;

            bool HasPaymentAccessToken = false;
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {

                if (!string.IsNullOrEmpty(FormId.ToString()))
                {
                    Form form = _formRepository.Get(FormId);
                    if (string.IsNullOrEmpty(form.PaymentAccessToken))
                    {
                        IQueryable<ACL> ACLs = _aclRepository.GetAll().Where(i => (i.EntityID == FormId || i.EntityID == form.OriginalId) && i.Role == "O");

                        User user = _userRepository.Get((long)ACLs.First().UserId);
                        if (string.IsNullOrEmpty(user.PaymentAccessToken))
                        {
                            //Do Tenant Payment Details lookup here or error
                        }
                        else
                        {
                            HasPaymentAccessToken = true;
                        }
                    }
                    else
                    {
                        HasPaymentAccessToken = true;
                    }
                    CurrentUnitOfWork.SaveChanges();

                    // f40fe1fb-7f5a-4aaf-92c5-0d0d50fbef5c Stripe
                    // var paymentformid = Guid.Parse("A7A1B9F8-FA11-4BAC-A8EB-31F81B10D53C");// StGeorge  
                    // paymentformid = Guid.Parse("F40FE1FB-7F5A-4AAF-92C5-0D0D50FBEF5C"); // Stripe

                    var paymentformid = SettingManager.GetSettingValueForApplication<Guid>(AppSettings.HostManagement.PaymentFormId);
                    if (form.TenantId != null)
                    {
                        paymentformid = SettingManager.GetSettingValueForTenant<Guid>(AppSettings.TenantManagement.PaymentFormId, (int)form.TenantId);
                    }

                    result = await GetSystemForm(new GetSystemFormForEdit()
                    {
                        Id = paymentformid,
                        OriginalId = paymentformid,
                        Version = "Live",
                        FormType = (int)FormType.PaymentForm
                    });

                }

                //if (HasPaymentAccessToken)
                //{
                //    result = await GetSystemForm(new GetSystemFormForEdit()
                //    {
                //        Id = Guid.Parse("A7A1B9F8-FA11-4BAC-A8EB-31F81B10D53C"),
                //        OriginalId = Guid.Parse("A7A1B9F8-FA11-4BAC-A8EB-31F81B10D53C"),
                //        Version = "Live",
                //        FormType = (int)FormType.PaymentForm
                //    });
                //}
                //else
                //{
                //    result = await GetSystemForm(new GetSystemFormForEdit()
                //    {
                //        Id = Guid.Parse("F749646F-2118-49BF-8C8B-3E0ED2B85520"),
                //        OriginalId = Guid.Parse("F749646F-2118-49BF-8C8B-3E0ED2B85520"),
                //        Version = "Live",
                //        FormType = (int)FormType.PaymentForm
                //    });
                //}


            }

            return result;
        }

        private ExpandoObject CreateDynamicPage(PageSettingsClass pageSettings)
        {
            dynamic DynamicPage = new ExpandoObject();
            DynamicPage.title = pageSettings.Label;
            DynamicPage.label = pageSettings.Label;
            DynamicPage.type = "sfapanel";
            DynamicPage.key = pageSettings.Key;
            //DynamicPage.input = false;
            DynamicPage.collapsible = pageSettings.Collapsible;
            DynamicPage.collapsed = pageSettings.Collapsed;
            DynamicPage.tableView = false;
            if (pageSettings.IsWizard)
            {
                DynamicPage.breadcrumb = pageSettings.Breadcrumb ?? "default";
                DynamicPage.breadcrumbClickable = pageSettings.BreadcrumbClickable;
            }
            return DynamicPage;
        }

        private SchemaButtonsClass CreateDynamicButtons(Form form, bool IsWizard)
        {
            JObject FormJObject = JObject.Parse(form.Schema);
            SchemaButtonsClass SchemaButtons = new SchemaButtonsClass();
            if (FormJObject.HasMatchingToken("buttons"))
            {
                SchemaButtons = JsonConvert.DeserializeObject<SchemaButtonsClass>(Convert.ToString(FormJObject.SelectToken("buttons")));
            }

            return SchemaButtons;
        }

        private Task<List<FormIOSchemaComponentClass>> FetchComponentsListFromSchema(string schema)
        {
            List<FormIOSchemaComponentClass> Components = new List<FormIOSchemaComponentClass>();
            JObject FormJObject = JObject.Parse(schema);
            try
            {
                FormJObject.SelectToken("components").ToList().ForEach(j =>
                {
                    FormIOSchemaComponentClass Component = JsonConvert.DeserializeObject<FormIOSchemaComponentClass>(Convert.ToString(j));
                    Components.Add(Component);
                });
            }
            catch { }

            return Task.FromResult(
                Components
            );
        }

        private Task<List<dynamic>> ParseFormIOComponents(Guid releaseId, List<FormIOSchemaComponentClass> Components, ref List<dynamic> FormComponents, ref string RulesCollection, ref string ScriptCollection)
        {
            List<dynamic> LocalFormComponents = FormComponents;
            string LocalRulesCollection = RulesCollection;
            string LocalScriptCollection = ScriptCollection;
            Dictionary<Type, int> TypeDict = new Dictionary<Type, int>
            {
                {typeof(string),0},
                {typeof(string[]),1},
                {typeof(bool?),2},
                {typeof(int),3},
                {typeof(object),4}, //This is for Dynamic properties i.e. defalutValue
				{typeof(DatePickerClass),5},
                {typeof(FormMapClass),6},
                {typeof(FormDataClass),7},
                {typeof(FormDateWidgetClass),8},
                {typeof(ValidationClass),9},
                {typeof(List<FormSubstitutionClass>),10},
                {typeof(List<FormlogicClass>),11},
                {typeof(List<FormValuesClass>),12},
                {typeof(List<FormIOSchemaComponentClass>),13},
                {typeof(List<FileUploadClass>),14},
                {typeof(List<AttrsClass>),15}
            };

            if (Components == null || Components.Count <= 0)
            {
                return Task.FromResult(
                    FormComponents
                );
            }
            else
            {
                Components.ForEach(i =>
                {
                    var PageComponents = new ExpandoObject() as IDictionary<string, object>;
                    if (i.Type != null)
                    {
                        if (i.Type == "nestedform")
                        {
                            List<FormSubstitutionClass> NestedSubstitutions = i.FNSubstitute ?? new List<FormSubstitutionClass>();

                            // Get from Release if has Release id
                            Form nestedform = new Form();
                            if (releaseId != Guid.Empty)
                            {
                                GetFormFromReleaseOutput rOutput = _projectManager.GetFormFromRelease(releaseId, Guid.Parse(i.FormId));
                                nestedform = rOutput.Form;
                            }
                            else
                            {
                                nestedform = _formRepository.FirstOrDefault(Guid.Parse(i.FormId));
                            }

                            if (nestedform != null)
                            {
                                Form SubstitutedForm = SubstituteComponentAndScriptNames(nestedform, NestedSubstitutions);
                                LocalRulesCollection += SubstitutedForm.RulesScript;
                                LocalScriptCollection += SubstitutedForm.Script;
                                ParseFormIOComponents(releaseId, FetchComponentsListFromSchema(SubstitutedForm.Schema).Result, ref LocalFormComponents, ref LocalRulesCollection, ref LocalScriptCollection);
                            }
                        }
                        else
                        {
                            foreach (PropertyInfo prop in i.GetType().GetProperties())
                            {
                                var Value = prop.GetValue(i, null);
                                switch (TypeDict[prop.PropertyType])
                                {
                                    case 1:
                                        var ArrValue = (Array)prop.GetValue(i, null);
                                        if (ArrValue != null && ArrValue.Length >= 1)
                                        {
                                            PageComponents.Add(prop.Name, Value);
                                        }
                                        break;
                                    case 0:
                                    case 2:

                                    default:
                                        if (!string.IsNullOrEmpty(Convert.ToString(Value)))
                                        {
                                            PageComponents.Add(prop.Name, Value);
                                        }
                                        break;
                                    case 3:
                                        if ((i.Type == "section" || i.Type == "sfapanel") && (int)Value >= 0)
                                        {
                                            // Section and Panel's border properties could be 0 (invisible border).
                                            PageComponents.Add(prop.Name, Value);
                                        }
                                        else if ((int)Value > 0)
                                        {
                                            // if other properties' value is 0, the property does not to add to schema.
                                            PageComponents.Add(prop.Name, Value);
                                        }
                                        break;
                                    case 4:
                                        var DynamicValue = (object)prop.GetValue(i, null);
                                        if (DynamicValue != null)
                                        {
                                            PageComponents.Add(prop.Name, Value);
                                        }
                                        break;
                                    case 5:
                                        var DateValue = (DatePickerClass)prop.GetValue(i, null);
                                        if (DateValue != null)
                                        {
                                            PageComponents.Add(prop.Name, Value);
                                        }
                                        break;
                                    case 6:
                                        var MapValue = (FormMapClass)prop.GetValue(i, null);
                                        if (MapValue != null)
                                        {
                                            PageComponents.Add(prop.Name, Value);
                                        }
                                        break;
                                    case 7:
                                        var DataValue = (FormDataClass)prop.GetValue(i, null);
                                        if (DataValue != null)
                                        {
                                            PageComponents.Add(prop.Name, Value);
                                        }
                                        break;
                                    case 8:
                                        var WidgetValue = (FormDateWidgetClass)prop.GetValue(i, null);
                                        if (WidgetValue != null)
                                        {
                                            PageComponents.Add(prop.Name, Value);
                                        }
                                        break;
                                    case 9:
                                        var ValidationValue = (ValidationClass)prop.GetValue(i, null);
                                        if (ValidationValue != null)
                                        {
                                            PageComponents.Add(prop.Name, Value);
                                        }
                                        break;
                                    case 10:
                                        var List1Value = (List<FormSubstitutionClass>)prop.GetValue(i, null);
                                        if (List1Value != null && List1Value.Count >= 1)
                                        {
                                            PageComponents.Add(prop.Name, Value);
                                        }
                                        break;
                                    case 11:
                                        var List2Value = (List<FormlogicClass>)prop.GetValue(i, null);
                                        if (List2Value != null && List2Value.Count >= 1)
                                        {
                                            PageComponents.Add(prop.Name, Value);
                                        }
                                        break;
                                    case 12:
                                        var List4Value = (List<FormValuesClass>)prop.GetValue(i, null);
                                        if (List4Value != null && List4Value.Count >= 1)
                                        {
                                            PageComponents.Add(prop.Name, Value);
                                        }
                                        break;
                                    case 13:
                                        var List3Value = (List<FormIOSchemaComponentClass>)prop.GetValue(i, null);
                                        if (List3Value != null && List3Value.Count >= 1)
                                        {
                                            List<dynamic> SubPageComponents = new List<dynamic>();
                                            ParseFormIOComponents(releaseId, List3Value, ref SubPageComponents, ref LocalRulesCollection, ref LocalScriptCollection);
                                            PageComponents.Add(prop.Name, SubPageComponents);
                                        }
                                        break;
                                    case 14:
                                        var List5Value = (List<FileUploadClass>)prop.GetValue(i, null);
                                        if (List5Value != null && List5Value.Count >= 1)
                                        {
                                            PageComponents.Add(prop.Name, Value);
                                        }
                                        break;
                                    case 15:
                                        var List6Value = (List<AttrsClass>)prop.GetValue(i, null);
                                        if (List6Value != null && List6Value.Count >= 1)
                                        {
                                            PageComponents.Add(prop.Name, Value);
                                        }
                                        break;
                                }
                            }
                            LocalFormComponents.Add(PageComponents);
                        }
                    }
                });
                FormComponents = LocalFormComponents;
                RulesCollection = LocalRulesCollection;
                ScriptCollection = LocalScriptCollection;
                return Task.FromResult(
                    LocalFormComponents
                );
            }
        }

        private Form SubstituteComponentAndScriptNames(Form form, List<FormSubstitutionClass> Substitutions)
        {
            Form _LocalForm = new Form
            {
                RulesScript = form.RulesScript,
                Schema = form.Schema,
                Script = form.Script
            };
            //Form _LocalForm = form;
            Substitutions.ForEach(i =>
            {
                if (!string.IsNullOrEmpty(i.FNPattern) && !string.IsNullOrEmpty(i.FNReplacement))
                {
                    if (_LocalForm.RulesScript != null)
                    {
                        _LocalForm.RulesScript = _LocalForm.RulesScript.Replace(i.FNPattern, i.FNReplacement);
                    }
                    if (_LocalForm.Schema != null)
                    {
                        _LocalForm.Schema = _LocalForm.Schema.Replace(i.FNPattern, i.FNReplacement);
                    }
                    if (_LocalForm.Script != null)
                    {
                        _LocalForm.Script = _LocalForm.Script.Replace(i.FNPattern, i.FNReplacement);
                    }
                }
            });
            return _LocalForm;
        }

        public async Task<Form> GetSystemForm(GetSystemFormForEdit input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                Form form = null;
                try
                {
                    if (input.Id != null)
                    {
                        // form = _formRepository.GetAll().Where(i => i.Id == input.Id && i.FormTypeId == input.FormType).FirstOrDefault();
                        form = _formRepository.GetAll().Where(i => i.Id == input.Id).FirstOrDefault();
                    }
                    else
                    {
                        int.TryParse(input.Version, out int VersionInt);
                        if (VersionInt != 0 && VersionInt.GetType() == typeof(int))
                        {
                            form = await GetFormByVersion((Guid)input.OriginalId, VersionInt);
                        }
                        else if (!string.IsNullOrEmpty(input.Version) && input.Version.GetType() == typeof(string))
                        {
                            form = await GetFormByVersion((Guid)input.OriginalId, null);
                        }
                        else
                        {
                            form = await GetFormByVersion((Guid)input.OriginalId, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException("Fetching Form failed because: " + ex);
                }
                CurrentUnitOfWork.SaveChanges();
                return form;
            }

        }

        //ADD
        private async Task<Form> GetFormByVersion(Guid OriginalId, int? iVersion)
        {
            Form form = null;
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                // Look for form based on Original Id and given version number. 
                // Or if no supplied version number take the live version

                var hasvalue = iVersion.HasValue;
                form = await _formRepository.FirstOrDefaultAsync(i =>
                    i.OriginalId == OriginalId && (hasvalue ? i.Version == iVersion : i.Version == i.CurrentVersion)
                );

                // If the form can't be found by the specified originalId and version number or the live version 
                // then get the next higest version of this originalid or if no higher version take any version
                if (form == null)
                    form = await _formRepository.FirstOrDefaultAsync(i =>
                    //    i.OriginalId == OriginalId && (hasvalue ? i.Version >= iVersion : true)
                         i.OriginalId == OriginalId && (hasvalue ? i.Version == iVersion : true)
                    );


                // If you can't find by originalId of any type i.e an Id has benn provided instead of an originalId  then look by Id
                if (form == null)
                    form = await _formRepository.FirstOrDefaultAsync(i =>
                        i.Id == OriginalId
                    );


            }
            return form;
        }

        // ACL checker manages permission
        // May be an anon user with an access token
        //[Authorize(Policy = "EditByRecordMatterId")]
        [AllowAnonymous]
        public async Task<string> Save(dynamic input)
        {

            // User can be anonymouse.
            // Application logic manages access
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            dynamic json = JsonConvert.DeserializeObject<dynamic>(input);
            bool IsOnlySubmit = DynamicUtility.IsPropertyExist(json, "IsOnlySubmit") ? json.IsOnlySubmit.Value : false;

            FormSaveDto formSaveDto = new FormSaveDto()
            {
                AccessToken = DynamicUtility.IsPropertyExist(json, "AccessToken") ? json.AccessToken : "",
                AnonAuthToken = DynamicUtility.IsPropertyExist(json, "AnonAuthToken") ? json.AnonAuthToken : "",
                Id = json.Id,
                RecordId = json.RecordId == 0 ? new Guid() : Guid.Parse(json.RecordId.ToString()),
                RecordMatterId = json.RecordMatterId == 0 ? new Guid() : Guid.Parse(json.RecordMatterId.ToString()),
                RecordMatterItemId = json.RecordMatterItemId == 0 ? new Guid() : Guid.Parse(json.RecordMatterItemId.ToString()),
                //SubmissionId = json.Submission.SubmissionId == 0 ? new Guid() : Guid.Parse(json.Submission.SubmissionId.ToString()),
                SubmissionId =  Convert.ToBoolean(IsOnlySubmit) ?
                                Guid.Parse(json.SubmissionId.ToString()) :
                                json.Submission.SubmissionId == 0 ?
                                Guid.NewGuid() :
                                Guid.Parse(json.Submission.SubmissionId.ToString()),

                Submission = IsOnlySubmit ? json.submission.data : json.Submission
            };

            for (int i = 0; i < 6; i++)
            {
                if (i == 5)
                {
                    throw new UserFriendlyException("This Record is currently locked while documents are being prepared. Please try saving again shortly.");
                }
                if (await _recordManager.IsLocked(formSaveDto.RecordId))
                {
                    await Task.Delay(6000);
                }
                else
                {
                    break;
                }
            }

            // MB MODIFIED
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var record = await _recordRepository.GetAll().FirstOrDefaultAsync(r => r.Id == formSaveDto.RecordId);
                if (record != null)
                {
                    record.Locked = DateTime.Now;
                }
                CurrentUnitOfWork.SaveChanges();
                unitOfWork.Complete();
            }

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = json.RecordMatterId, UserId = AbpSession.UserId, AccessToken = formSaveDto.AccessToken, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {

                RecordSet recordSet = null;

                Guid folderId = Guid.Parse("00000000-0000-0000-0000-000000000000");
                Guid parentfolderId = Guid.Parse("00000000-0000-0000-0000-000000000000");
                var recordname = string.Empty;
                var recordmattername = string.Empty;
                var job = _appJobRepository.FirstOrDefault(i => i.EntityId == formSaveDto.Id);
                if (job != null)
                {
                    Apps.Dtos.CreateOrEditAppJobDto jJobdata = JsonConvert.DeserializeObject<Apps.Dtos.CreateOrEditAppJobDto>(job.Data);
                    if (jJobdata != null)
                    {
                        if (jJobdata.RecordMatter != null)
                        {
                            var recordmatter = jJobdata.RecordMatter.FirstOrDefault();
                            if (recordmatter != null)
                            {
                                string submission = Convert.ToString(formSaveDto.Submission);

                                recordname = GetFieldValue(submission, recordmatter.RecordName);
                                recordmattername = GetFieldValue(submission, recordmatter.RecordMatterName);

                                if (recordmatter.FolderId.HasValue)
                                {
                                    folderId = (Guid)recordmatter.FolderId;

                                    var pf = _folderRepository.GetAll().FirstOrDefault(i => i.Id == folderId)?.ParentId;
                                    parentfolderId = pf.HasValue ? pf.Value : Guid.Parse("00000000-0000-0000-0000-000000000000");

                                }

                            }
                        }
                    }
                }

                long? userId = AbpSession.UserId;
                int? tenantId = AbpSession.TenantId;

                if (userId == null)
                {
                    // Get the OriginalID of the Form
                    var foriginalid = _formRepository.FirstOrDefault(e => e.Id == formSaveDto.Id)?.OriginalId;
                    // Get the Owner and tenant of the Form
                    var acl = _aclRepository.FirstOrDefault(i => i.EntityID == foriginalid && i.Role == "O").UserId;

                    userId = acl;

                    var user = UserManager.Users.FirstOrDefault(i => i.Id == userId);
                    if (user != null)
                    {
                        tenantId = user.TenantId;
                    }

                }

                JObject fileData = JsonConvert.DeserializeObject<JObject>(formSaveDto.Submission.ToString());
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
                            if (fileData[MainKey][0].Value<bool>("file") == true)
                            {
                                hasFiles = true;
                            }
                            else if (fileData[MainKey][0].Value<bool>("repeat") == true)
                            {
                                //JToken repeatItems = fileData[MainKey];
                                hasFiles = FindUploadFileinRepeat(hasFiles, fileData[MainKey]);
                                //foreach (JObject repeatItem in repeatItems.Children())
                                //{
                                //    foreach (KeyValuePair<string, JToken> subkey in repeatItem)
                                //    {
                                //        string SubKey = subkey.Key;
                                //        if (!string.IsNullOrEmpty(SubKey) && repeatItem[SubKey].Type == JTokenType.Array && repeatItem[SubKey].HasValues)
                                //        {
                                //            if (repeatItem[SubKey][0].Value<bool>("file") == true)
                                //            {
                                //                hasFiles = true;
                                //            }
                                //            else if (repeatItem[SubKey][0].Value<bool>("repeat") == true)
                                //            {
                                //                hasFiles = FindUploadFileinRepeat(hasFiles, repeatItem[SubKey]);
                                //            }
                                //        }
                                //    }
                                //}
                            }

                        }
                        catch (Exception e) { }
                    }
                }


                recordSet = new RecordSet()
                {
                    AnonAuthToken = formSaveDto.AnonAuthToken,
                    UserId = userId,
                    ACLPermission = "O",
                    HasFiles = hasFiles,
                    OrganizationId = /*j.OrganizationID != null ? j.OrganizationID : */null,
                    TenantId = tenantId,
                    SubmissionData = formSaveDto.Submission.ToString(),
                    RecordId = formSaveDto.RecordId != null ? Guid.Parse(formSaveDto.RecordId.ToString()) : Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    RecordName = recordname ?? "Default",
                    RecordMatterId = formSaveDto.RecordMatterId != null ? Guid.Parse(formSaveDto.RecordMatterId.ToString()) : Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    RecordMatterName = recordmattername ?? "Default",
                    RecordMatterItemId = formSaveDto.RecordMatterItemId != null ? Guid.Parse(formSaveDto.RecordMatterItemId.ToString()) : Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    RecordMatterItemGroupId = formSaveDto.RecordMatterItemId != null ? Guid.Parse(formSaveDto.RecordMatterItemId.ToString()) : Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    SubmissionId = formSaveDto.SubmissionId != null ? Guid.Parse(formSaveDto.SubmissionId.ToString()) : Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    FormId = formSaveDto.Id,
                    FolderId = folderId, //Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    ParentFolderId = parentfolderId,
                    FolderName = "Your Records",
                    FolderType = "R"
                };

                bool HasACLAccess = true;
                bool saveRecordMatterItem = true;
                string AccessError = null;

                //if (hasFiles) 
                //{ 
                //	_recordMatterItemRepository.GetAll().Any(rmi => rmi.Id==)
                //}

                // If this is a Project do not save the recordmatteritem. These are only update on the submit process
                if (_projectRepository.GetAll().Any(p => p.RecordId == formSaveDto.RecordId))
                {
                    if (hasFiles)
                    {
                        var rmi = _recordMatterItemRepository.FirstOrDefault(
                               m => m.RecordMatterId == formSaveDto.RecordMatterId
                               );
                        if (rmi != null)
                        {
                            saveRecordMatterItem = false;
                            recordSet.RecordMatterItemGroupId = rmi.GroupId;
                        }
                    }
                    else
                    {
                        saveRecordMatterItem = false;
                    }

                }

                (recordSet, HasACLAccess, AccessError) = await _recordManager.SaveRecordSet(recordSet, saveRecordMatterItem);

                // SAVE UPLOADED FILE OBJECTS
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
                                            AccessToken = formSaveDto.AnonAuthToken // DynamicUtility.IsPropertyExist(input, "AnonAuthToken") ? input.AnonAuthToken : null
                                        });
                                    }
                                }
                            }
                            else if (fileData[MainKey][0].Value<bool>("repeat") == true)
                            {
                                /////start
                                string AccessToken = formSaveDto.AnonAuthToken; // DynamicUtility.IsPropertyExist(input, "AnonAuthToken") ? input.AnonAuthToken : null;
                                SaveUploadFileInRepeatPanel(fileData[MainKey], recordSet, AccessToken);
                                //JToken repeatItems = fileData[MainKey];
                                //foreach (JObject repeatItem in repeatItems.Children())
                                //{
                                //    foreach (KeyValuePair<string, JToken> subkey in repeatItem)
                                //    {
                                //        string SubKey = subkey.Key;
                                //        if (!string.IsNullOrEmpty(SubKey) && repeatItem[SubKey].Type == JTokenType.Array && repeatItem[SubKey].HasValues)
                                //        {
                                //            if (repeatItem[SubKey][0].Value<bool>("file") == true)
                                //            {
                                //                foreach (JToken file in (JArray)repeatItem[SubKey])
                                //                {
                                //                    if (file.HasValues)
                                //                    {
                                //                        FileDto FileDto = JsonConvert.DeserializeObject<FileDto>(file.ToString());
                                //                        _filesManager.SaveFile(new SaveFileDto()
                                //                        {
                                //                            RecordId = recordSet.RecordId,
                                //                            RecordMatterId = recordSet.RecordMatterId,
                                //                            RecordMatterItemGroupId = recordSet.RecordMatterItemGroupId,
                                //                            File = FileDto,
                                //                            AccessToken = DynamicUtility.IsPropertyExist(input, "AnonAuthToken") ? input.AnonAuthToken : null
                                //                        });
                                //                    }
                                //                }
                                //            }
                                //        }
                                //    }
                                //}
                                ////End

                            }
                        }
                        catch { }
                    }
                }

                formSaveDto.AnonAuthToken = recordSet.AnonAuthToken;
                formSaveDto.RecordId = recordSet.RecordId;
                formSaveDto.RecordMatterId = recordSet.RecordMatterId;
                formSaveDto.RecordMatterItemId = recordSet.RecordMatterItemId;
                formSaveDto.SubmissionId = recordSet.SubmissionId;


                // MB MODIFIED
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                    {
                        try
                        {
                            var record = _recordRepository.GetAll().FirstOrDefault(r => r.Id == formSaveDto.RecordId);
                            if (record != null)
                            {
                                record.Locked = null;
                            }

                            CurrentUnitOfWork.SaveChanges();
                            unitOfWork.Complete();
                        }
                        catch { }
                    }
                }

                return JsonConvert.SerializeObject(formSaveDto);

            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

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
        //int 
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

        private string GetFieldValue(string data, string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                var result = string.Empty;
                var jdata = JsonConvert.DeserializeObject<JObject>(data);
                Regex Pattern = new Regex("{.*?}", RegexOptions.IgnoreCase);
                var matches = Pattern.Matches(input);
                matches.ToList().ForEach(i1 =>
                {
                    result = StringUtility.ReplaceBracketValuesJSON(i1, jdata, input);
                });
                return string.IsNullOrEmpty(result) ? input : result;
            }
            else
            {
                return "Default";
            }
        }
        //new , old	
        public JToken GetDataDiff(string dataA, string dataB)
        {
            JToken result = JToken.Parse("{}");
            dataA = string.IsNullOrEmpty(dataA) ? "{}" : dataA;
            dataB = string.IsNullOrEmpty(dataB) ? "{}" : dataB;
            var j1 = JToken.Parse(dataA);
            var j2 = JToken.Parse(dataB);

            var jdp = new JsonDiffPatchDotNet.JsonDiffPatch();
            result = jdp.Diff(j1, j2);

            // Changed library to fix Form save error
            //result = JsonDifferentiator.Differentiate(j2, j1);
            return result;
        }

        public async Task SetCurrent(FormVersionDto formVersionDto)
        {

            if (_ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = (Guid)formVersionDto.OriginalId, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId }).IsAuthed)
            {

                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
                var forms = _formRepository.GetAll().Where(i => i.OriginalId == formVersionDto.OriginalId);

                //compare the form difference between old alive form and new alive form	
                var formOld = await _formRepository.FirstOrDefaultAsync(i =>
                    i.OriginalId == formVersionDto.OriginalId && i.Version == i.CurrentVersion);
                var formNew = await _formRepository.FirstOrDefaultAsync(i =>
                    i.OriginalId == formVersionDto.OriginalId && i.Version == formVersionDto.Version);
                int previousV = formOld.CurrentVersion;

                var newData = formNew.Schema;
                var oldData = formOld.Schema;
                var jDiff = GetDataDiff(formNew.Schema, formOld.Schema);

                foreach (Form form in forms)
                {
                    form.CurrentVersion = formVersionDto.Version;
                    await _formRepository.UpdateAsync(form);
                }

                //update the version history entity	
                var rma = new EntityVersionHistory()
                {
                    Data = jDiff == null ? "{}" : jDiff.ToString(),
                    Name = formNew.Name,
                    Description = formVersionDto.VersionDes,
                    Version = formNew.Version,
                    PreviousVersion = previousV, //live version	
                    EntityId = formNew.Id,
                    UserId = AbpSession.UserId,
                    TenantId = AbpSession.TenantId,
                    VersionName = formVersionDto.VersionName,
                    PreviousData = oldData,
                    NewData = newData,
                    Type = "Form",

                };
                await _entityVersionHistoryRepository.InsertAsync(rma);

                ACL aCL = new ACL()
                {
                    UserId = AbpSession.UserId,
                    TenantId = AbpSession.TenantId,
                    EntityID = rma.Id,
                    Type = "EntityVersionHistory",
                    Role = "O",
                };
                await _ACLManager.AddACL(aCL);

            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

        public async Task<bool> Run(dynamic input)
        {

            // May be run anonymously
            CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant);

            dynamic output = new ExpandoObject();

            JObject jInput = JsonConvert.DeserializeObject<JObject>(input, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
            try
            {
                jInput.Add("Id", jInput.SelectToken("id"));
                jInput.Add("RecordId", jInput.SelectToken("submission.data.RecordId"));
                jInput.Add("RecordMatterId", jInput.SelectToken("submission.data.RecordMatterId"));
                jInput.Add("RecordMatterItemId", jInput.SelectToken("submission.data.RecordMatterItemId"));
                jInput.Add("AccessToken", jInput.SelectToken("submission.data.AccessToken"));
                jInput.Add("SubmissionId", jInput.SelectToken("submission.data.SubmissionId"));
                jInput.Add("IsOnlySubmit", true);

                dynamic jsonString = JsonConvert.SerializeObject(jInput);

                string saveResult = await Save(jsonString);
            }
            catch (Exception ex)
            {
            }

            // MB MODIFIED
            JToken jRId = jInput.SelectToken("submission.data.RecordId");
            if (jRId != null)
            {
                var rId = new Guid(jRId.Value<string>());
                if (await _recordManager.IsLocked(rId))
                {
                    throw new UserFriendlyException("This Record is currently locked while documents are being prepared. Please try again shortly.");
                }
                else
                {

                    using (var unitOfWork = _unitOfWorkManager.Begin())
                    {
                        var record = await _recordRepository.GetAll().FirstOrDefaultAsync(r => r.Id == rId);
                        if (record != null)
                        {
                            record.Locked = DateTime.Now;
                        }
                        CurrentUnitOfWork.SaveChanges();
                        unitOfWork.Complete();
                    }

                }
            }



            NormaliseDates(jInput);

            #region Image Render Issue with documents - 10297
            try
            {
                // Create a deep copy of the JObject
                JObject inputData = (JObject)jInput.DeepClone();

                // Check if the "submission" object and "data" object exist
                if (inputData["submission"] != null && inputData["submission"]["data"] != null)
                {
                    // Extract the "imageUpload" array if it exists
                    JArray imageUploadArray = (JArray)inputData["submission"]["data"]["imageUpload"];

                    // Check if the "imageUpload" array is present
                    if (imageUploadArray != null)
                    {
                        // Iterate through the array and modify the "url" values
                        foreach (JObject imageUpload in imageUploadArray)
                        {
                            // Get the current "url" value
                            string currentUrl = (string)imageUpload["url"];
                            if (currentUrl.StartsWith("data:"))
                            {
                                // Remove the "data:," prefix
                                string modifiedUrl = currentUrl.Replace("data:", "");
                                // Update the "url" value with the modified URL
                                imageUpload["url"] = modifiedUrl;
                            }
                        }
                    }
                    // Update the modified InputeData URL back to a JInput
                    jInput = inputData;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
            }
            #endregion



            Guid? formId = null; dynamic submissionData = null;

            JToken jFormId = jInput.Property("id");
            if (jFormId == null)
            {

                // If only supply a rmi					
                JToken jRecordMatterId = jInput.Property("RecordMatterId");
                if (jRecordMatterId != null)
                {
                    var rmid = new Guid(Convert.ToString(jInput.Property("RecordMatterId").Value));
                    var rm = _recordMatterRepository.GetAll()
                        .Select(rm => new { rm.Id , rm.FormId, rm.Data, rm.AccessToken})
                        .FirstOrDefault(i => i.Id == rmid);
                    if (rm != null)
                    {
                        formId = rm.FormId;
                        submissionData = rm.Data;
                        output.AnonAuthToken = string.IsNullOrEmpty(rm.AccessToken) ? null : rm.AccessToken;
                    }
                }
            }
            else
            {
                formId = new Guid(Convert.ToString(jInput.Property("id").Value));
                submissionData = JsonConvert.SerializeObject(jInput["submission"]["data"]);
                output.AnonAuthToken = jInput.Property("AnonAuthToken").Value.ToString();
            }
 
            if (formId.HasValue)
            {
                Form form = await GetFormByVersion((Guid)formId, null);
                if (form != null)
                {
                    if (form.IsEnabled)
                    {
                        // pass voucher amount
                        dynamic Data = JsonConvert.DeserializeObject<ExpandoObject>(submissionData);
                        output.VoucherAmount = DynamicUtility.IsPropertyExist(Data, "VoucherAmount") ? Convert.ToDecimal(Data.VoucherAmount) : 0; ;
                        
                        output.Job = _appJobRepository.GetAll()
                            .Where(i => i.EntityId == formId).ToList();

                        AppClass appClass = new AppClass()
                        {
                            Id = formId.ToString(),
                            DataURL = null,
                            data = submissionData
                        };

                        output.FormId = appClass.Id;
                        output.data = string.IsNullOrEmpty(appClass.data) ? _appRepository.Get(new Guid(input.Id)).Data : appClass.data;
                        JObject data = JsonConvert.DeserializeObject<JObject>(output.data);

                        output.summaryTableHTML = jInput.ContainsKey("summarytablehtml") ? jInput["summarytablehtml"].ToString() : "";

                        // Get RecordId if supplied or generate new one
                        // Get New RecordMatterId 
                        var recordId = GetJArrayValue(data, "RecordId");
                        if (!string.IsNullOrEmpty(recordId))
                        {
                            if (!_ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = new Guid(recordId), UserId = AbpSession.UserId, AccessToken = (string)output.AnonAuthToken, TenantId = AbpSession.TenantId }).IsAuthed)
                            {
                                throw new UserFriendlyException("Not Authorised");
                            }
                        }

                        var recordMatterId = GetJArrayValue(data, "RecordMatterId");
                        if (!string.IsNullOrEmpty(recordMatterId))
                        {
                            if (!_ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = new Guid(recordMatterId), UserId = AbpSession.UserId, AccessToken = (string)output.AnonAuthToken, TenantId = AbpSession.TenantId }).IsAuthed)
                            {
                                throw new UserFriendlyException("Not Authorised");
                            }
                        }

                        output.LockOnBuild = form.LockOnBuild;
                        _documentAppService.Automate(output);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        private void NormaliseDates(JObject input)
        {
            AppClass appClass = input.ToObject<AppClass>();

            if (_appRepository.GetAll().Any(a => a.Id == new Guid(appClass.Id)))
            {
                var app = _appRepository.GetAll().First(a => a.Id == new Guid(appClass.Id));
                if (app != null)
                {
                    foreach (JToken token in input.Descendants())
                    {
                        WalkNode(token, null, prop =>
                        {
                            DateTime temp;
                            if (DateTime.TryParse(Convert.ToString(prop.Value), out temp))
                            {
                                DateTime dt = DateTime.Parse(prop.Value.ToString());
                                var newvalue = dt.ToString("yyyy-MM-ddTHH:mm:ss");
                                prop.Value = newvalue;
                            }
                        });
                    }
                }
            }
        }

        private void WalkNode(JToken node, Action<JObject> objectAction = null, Action<JProperty> propertyAction = null)
        {
            if (node.Type == JTokenType.Object)
            {
                if (objectAction != null) objectAction((JObject)node);

                foreach (JProperty child in node.Children<JProperty>())
                {
                    if (propertyAction != null) propertyAction(child);
                    WalkNode(child.Value, objectAction, propertyAction);
                }
            }
            else if (node.Type == JTokenType.Array)
            {
                foreach (JToken child in node.Children())
                {
                    WalkNode(child, objectAction, propertyAction);
                }
            }
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

        private async Task<bool> IsFormEnabledAsync(Guid Id)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
            Form form = await GetFormByVersion((Guid)Id, null);

            //Form form = await _formRepository.GetAsync(Id);
            return form.IsEnabled;
        }

        public async Task<bool> Move(MoveFolderDto moveFolderDto)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = moveFolderDto.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
                moveFolderDto.UserId = AbpSession.UserId;
                return await _folderManager.Move(moveFolderDto);
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

        public async Task<bool> ToggleForm(Guid Id, bool Toggle)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {
                //_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
                Form form = await _formRepository.GetAsync(Id);
                form.IsEnabled = Toggle;
                await _formRepository.UpdateAsync(form);
                return true;
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }


        }

        public async Task SaveScript(FormScriptDto formScriptDto)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = formScriptDto.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
                (await _formRepository.FirstOrDefaultAsync(formScriptDto.Id)).Script = formScriptDto.Script;
                await _formRepository.UpdateAsync(await _formRepository.FirstOrDefaultAsync(formScriptDto.Id));

            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

        public async Task SaveRules(FormRulesDto formRulesDto)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = formRulesDto.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
                (await _formRepository.FirstOrDefaultAsync(formRulesDto.Id)).RulesScript = formRulesDto.RulesScript;
                await _formRepository.UpdateAsync(await _formRepository.FirstOrDefaultAsync(formRulesDto.Id));
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

        public async Task<GetNZBNEntitiesForView> GetNZBNEntities(string searchterm)
        {

            GetNZBNEntitiesForView result = new GetNZBNEntitiesForView();

            using (var client = new HttpClient())
            {

                HttpResponseMessage response = null;

                try
                {

                    // Trim Entity
                    searchterm = searchterm.Replace(" Limited", "");
                    searchterm = searchterm.Replace(" limited", "");

                    //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _NZBNConnection.Value.AccessToken);
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _NZBNConnection.Value.AccessToken);

                    client.DefaultRequestHeaders
                            .Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //response = client.GetAsync(_NZBNConnection.Value.Url + "/entities?search-term=" + searchterm).Result;  // Blocking call!  
                    response = client.GetAsync(_NZBNConnection.Value.Url + "?search-term=" + searchterm).Result;  // Blocking call!  
                    var responseresult = response.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<GetNZBNEntitiesForView>(responseresult.Result.ToString());


                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException(ex.Message);
                }

                return result;


            }
        }

        public async Task<NZBNEntity> GetNZBNEntity(string nzbn)
        {

            NZBNEntity result = new NZBNEntity();

            using (var client = new HttpClient())
            {

                HttpResponseMessage response = null;

                try
                {

                    //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _NZBNConnection.Value.AccessToken);
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _NZBNConnection.Value.AccessToken);
                    client.DefaultRequestHeaders
                            .Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));


                    response = client.GetAsync(_NZBNConnection.Value.Url + "/" + nzbn).Result;  // Blocking call!  
                    var responseresult = response.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<NZBNEntity>(responseresult.Result.ToString());

                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException(ex.Message);
                }

                return result;

            }

        }
    }
}