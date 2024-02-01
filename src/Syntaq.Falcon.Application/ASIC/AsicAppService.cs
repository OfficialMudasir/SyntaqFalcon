using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Syntaq.Falcon.ASIC.Exporting;
using Syntaq.Falcon.ASIC.Dtos;
using Syntaq.Falcon.Dto;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using Syntaq.Falcon.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Syntaq.Falcon.Utility;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Hangfire;
using Syntaq.Falcon.MultiTenancy;
using Abp.Net.Mail;
using Abp.Runtime.Session;
using Syntaq.Falcon.Apps.Dtos;
using Syntaq.Falcon.Apps.Exporting;
using Syntaq.Falcon.Apps;
using Syntaq.Falcon.Documents.Models;
using Syntaq.Falcon.Records;
using Microsoft.AspNetCore.Mvc;
using Abp.Domain.Uow;
using Microsoft.AspNetCore.Http;
using Syntaq.Falcon.Authorization.Users;
using Abp.Authorization.Users;
using System.Net;
using Syntaq.Falcon.Url;
using Abp.MultiTenancy;
using Abp.Timing;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Forms.Dtos;
using System.IO;
using System.IO.Compression;
using Syntaq.Falcon.Projects;
using Syntaq.Falcon.Submissions;
using Microsoft.Extensions.Options;
using Syntaq.Falcon.Web;
using System.Text.RegularExpressions;

namespace Syntaq.Falcon.ASIC
{
    [AbpAuthorize(AppPermissions.Pages_Asic)]
    public class AsicAppService : FalconAppServiceBase, IAsicAppService
    {
        private readonly IRepository<Asic, Guid> _asicRepository;
        private readonly IAsicExcelExporter _asicExcelExporter;
        private readonly IEmailSender _emailSender;

        private readonly IAppsAppService _appsAppService;
        private readonly RecordManager _recordManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly UserManager _userManager;
        private readonly IWebUrlService _webUrlService;
        private readonly ITenantCache _tenantCache;
        private readonly ACLManager _ACLManager;
        private readonly AsicDocumentAssemblyManager _asicDocumentAssemblyManager;
        private readonly IFormsAppService _formAppService;
        private readonly IRepository<RecordMatterItem, Guid> _recordMatterItemRepository;
        private readonly IProjectsAppService _projectsAppService;
        private readonly IRepository<Submission, Guid> _submissionRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IOptions<GetEdgeConfig> _getEdgeConfig;
        private readonly IRepository<User, long> _lookup_userRepository;

        public AsicAppService(AsicDocumentAssemblyManager asicDocumentAssemblyManager, ACLManager aclManager, IAppsAppService appsAppService, UserManager userManager, IUnitOfWorkManager unitOfWorkManager, RecordManager recordManager, IRepository<Asic, Guid> asicRepository, IEmailSender emailSender, IAsicExcelExporter asicExcelExporter, IWebUrlService webUrlService, ITenantCache tenantCache, IFormsAppService formAppService, IRepository<RecordMatterItem, Guid> recordMatterItemRepository, IProjectsAppService projectsAppService, IRepository<Submission, Guid> submissionRepository, IOptions<GetEdgeConfig> getEdgeConfig, IRepository<Tenant> tenantRepository, IRepository<User, long> lookup_userRepository)
        {
            _ACLManager = aclManager;
            _appsAppService = appsAppService;
            _asicRepository = asicRepository;
            _asicExcelExporter = asicExcelExporter;
            _emailSender = emailSender;
            _recordManager = recordManager;
            _unitOfWorkManager = unitOfWorkManager;
            _userManager = userManager;
            _webUrlService = webUrlService;
            _tenantCache = tenantCache;
            _asicDocumentAssemblyManager = asicDocumentAssemblyManager;
            _formAppService = formAppService;
            _recordMatterItemRepository = recordMatterItemRepository;
            _projectsAppService = projectsAppService;
            _submissionRepository = submissionRepository;

            _getEdgeConfig = getEdgeConfig;
            _tenantRepository = tenantRepository;
            _lookup_userRepository = lookup_userRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration)]
        public async Task<PagedResultDto<GetAsicForViewDto>> GetAll(GetAllAsicInput input)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            var filteredAsic = _asicRepository.GetAll()
                          .WhereIf(AbpSession.TenantId.HasValue, e => e.TenantId == (int)AbpSession.TenantId)
                          .Where(e => (e.LastModificationTime ?? e.CreationTime) >= input.StartDateFilter && (e.LastModificationTime ?? e.CreationTime) <= input.EndDateFilter);
                          
            if (!string.IsNullOrWhiteSpace(input.TenantNameFilter?.Trim()))
            {
                var tenantId = _tenantRepository.FirstOrDefault(t => t.Name == input.TenantNameFilter.Trim())?.Id;
                filteredAsic = filteredAsic.Where(e => e.TenantId == tenantId);
            }

            var pagedAndFilteredAsic = filteredAsic
                .OrderByDescending(i => i.RequestId)
                .PageBy(input);

            var asic = from o in pagedAndFilteredAsic
                       join o1 in _tenantRepository.GetAll() on o.TenantId equals o1.Id into j1
                       from s1 in j1.DefaultIfEmpty()
                       //join u1 in _lookup_userRepository.GetAll() on s1.CreatorUser
                       select new
                       {
                           o.TenantId,
                           o.RequestId,
                           o.CreatorUserId,
                           o.CreationTime,
                           o.LastModificationTime,
                           tenantName = s1 == null ? " " : s1.Name.ToString(),
                           o.Data,
                           o.Status,
                           Id = o.Id
                       };

            var totalCount = await filteredAsic.CountAsync();

            var dbList = await asic.ToListAsync();
            var results = new List<GetAsicForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetAsicForViewDto()
                {
                    Asic = new AsicDto
                    {
                        TenantId = o.TenantId,
                        RequestId = o.RequestId,
                        LastModificationTime = o.LastModificationTime == null ? o.CreationTime : (DateTime)o.LastModificationTime,
                        Status = o.Status,
                        Id = o.Id
                    },
                    TenantName = o.tenantName,
                    CompanyName = getCompanyName(o.Data),
                    //UserName = _lookup_userRepository.FirstOrDefault(x => x.Id == o.CreatorUserId)?.Name
                    UserName = _lookup_userRepository.FirstOrDefault(x => x.Id == o.CreatorUserId)?.EmailAddress,
                };

                results.Add(res);
            }
            return new PagedResultDto<GetAsicForViewDto>(
                totalCount,
                results
            );
        }

        private string getCompanyName(string companyData)
        {
            Form201Dto formdata = JsonConvert.DeserializeObject<Form201Dto>(companyData);
            

            if (MapYN(formdata?.Co_Name_As_ACN_yn?.Co_Name_As_ACN_yn) == "Y")
            {
                return String.Concat(formdata?.Co_ACN_msk?.ToUpper(), " ", formdata?.Co_Legal_Element_cho?.value?.ToUpper());
            }
            else
            {
                return String.Concat(formdata?.Co_Name_txt?.ToUpper(), " ", formdata?.Co_Legal_Element_cho?.value?.ToUpper());
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Asic_Create, AppPermissions.Pages_Asic_Edit)]
        public ASIC201Dto MapTo201(Form201Dto input)
        {
            try
            {
                ASIC201Dto asicData = new ASIC201Dto();
                asicData.identifier = Convert.ToString(input?.SubmissionId);

                asicData.company_id = MapCompanyId(input);

                if (MapYN(input?.CO_410_yn?.CO_410_yn?.ToString()) == "Y")
                {
                    asicData.reservation = MapReservation(input);
                }

                //mandatory if name_identical = Y 
                if (MapYN(input?.Co_RBN_yn?.Co_RBN_yn?.ToString()) == "Y")
                {
                    asicData.business = MapBusiness(input);
                }
                //asicData.members_amount //company_class = LMGT, current company_class =LMSH hard code
                asicData.registered_office = MapRegistered_office(input);

                //asic.standard_hours  when company_type= APUB not allowed, because company_type is hard code as APTY
                //asic.officehour

                asicData.principal_place = MapPrincipalPlace(input);

                if (MapYN(input?.Co_Ult_Hold_yn?.Co_Ult_Hold_yn) == "Y")
                {
                    asicData.ultimate_holding = MapUltimateHolding(input);
                }

                //directors or secretary can be map to officer
                asicData.officers = MapOfficer(input);

                //share members
                asicData.members = MapShareholder(input);

                asicData.share_class = MapShareClassList(input);

                //our form don't need nonshare_member, because company_class =LMGT
                asicData.applicant = MapApplicant(input);

                asicData.admin = MapAdmin(input);

                //How to pass token in the deployment mode
                asicData.token = _getEdgeConfig.Value.XAuthEdge;

                asicData.test_transmission = "N";
                if (input.is_test_transmission)
                {
                    asicData.test_transmission = "Y";
                }

                if (MapYN(input?.Co_Name_As_ACN_yn?.Co_Name_As_ACN_yn) == "Y")
                {
                    asicData.company_full_name = "A.C.N. XXX XXX XXX " + input?.Co_Legal_Element_cho?.value?.ToString()?.ToUpper();
                }
                else
                {
                    asicData.company_full_name = input?.Co_Name_txt?.ToString()?.ToUpper() + " " + input?.Co_Legal_Element_cho?.value?.ToString()?.ToUpper();

                }

                return asicData;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(Convert.ToString(e.Message), Convert.ToString(e.Source));
            }
        }


        //check companyName
        [AbpAuthorize(AppPermissions.Pages_Asic_Create, AppPermissions.Pages_Asic_Edit)]
        public async Task<JToken> CheckName(string companyName)
        {
            HttpResponseMessage response = null;
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("X-Auth-Edge", _getEdgeConfig.Value.XAuthEdge);

                    client.DefaultRequestHeaders
                            .Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));


                    JObject jsonpayload = new JObject();

                    jsonpayload["company_name"] = companyName.ToUpper();

                    string FormJson = JsonConvert.SerializeObject(jsonpayload);

                    response = client.PostAsync($"{_getEdgeConfig.Value.GetEdgeAPI}/checkName", new StringContent(FormJson, Encoding.UTF8, "application/json")).Result;  // Blocking call!  
                    var responseresult = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<JToken>(responseresult);
                    return result;
                }
                catch (Exception e)
                {
                    throw new UserFriendlyException(e.Message);
                }
            }
        }

        //trigger an submission workflow or send email of finish status
        [AbpAuthorize(AppPermissions.Pages_Asic_Create, AppPermissions.Pages_Asic_Edit)]
        public async Task<Submit201Output> Submit201(dynamic input)
        {

            if (!DynamicUtility.IsPropertyExist(input, "Id"))
            {
                throw new UserFriendlyException("A Form Id must be provided");
            }

            // BUILD FORMSAVE DTO
            FormSaveDto formSaveDto = new FormSaveDto()
            {
                AccessToken = DynamicUtility.IsPropertyExist(input, "AccessToken") ? input.AccessToken : JwtSecurityTokenProvider.GenerateToken(DateTime.Now.ToString(), 4),
                AnonAuthToken = DynamicUtility.IsPropertyExist(input, "AnonAuthToken") ? input.AnonAuthToken : JwtSecurityTokenProvider.GenerateToken(DateTime.Now.ToString(), 4),
                Id = input.Id,
                RecordId = DynamicUtility.IsPropertyExist(input, "RecordId") ? Guid.Parse(Convert.ToString(input.RecordId)) :  Guid.NewGuid(),
                RecordMatterId = DynamicUtility.IsPropertyExist(input, "RecordMatterId") ? Guid.Parse(Convert.ToString(input.RecordMatterId)): Guid.NewGuid(),
                RecordMatterItemId = DynamicUtility.IsPropertyExist(input, "RecordMatterItemId") ? Guid.Parse(Convert.ToString(input.RecordMatterItemId)): Guid.NewGuid(),
                SubmissionId = input.Submission.SubmissionId == null ? Guid.NewGuid() : Guid.Parse(Convert.ToString(input.Submission.SubmissionId)),
                Submission = input.Submission
            };

            //add exemption in output
            try
            {
                JsonConvert.DeserializeObject<Form201Dto>(Convert.ToString(formSaveDto.Submission));
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(Convert.ToString(e.Message), Convert.ToString(e.Source));
            }

            Form201Dto formdata = JsonConvert.DeserializeObject<Form201Dto>(Convert.ToString(formSaveDto.Submission));

            if (formdata == null)
            {
                throw new UserFriendlyException("Submission Data cannot be empty");
            }

            // AUTH CHECK
            if (!_ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = new Guid(Convert.ToString(formdata.RecordMatterId)), UserId = AbpSession.UserId, AccessToken = formdata.AccessToken, TenantId = AbpSession.TenantId }).IsAuthed)
            {
                throw new UserFriendlyException("Not Authorised");
            }

            // SAVE THE RECORDSET
            var saveresult = Task.Run(async () => await _formAppService.Save(JsonConvert.SerializeObject(formSaveDto))).Result;

            formSaveDto = JsonConvert.DeserializeObject<FormSaveDto>(Convert.ToString(saveresult));

            //UPDATE FORMDATA WITH UPDATED IDs
            formdata.FormId = formSaveDto.Id;
            formdata.RecordId = formSaveDto.RecordId;
            formdata.RecordMatterId = formSaveDto.RecordMatterId;
            formdata.RecordMatterItemId = formSaveDto.RecordMatterItemId;
            formdata.SubmissionId = formSaveDto.SubmissionId;
            formdata.AccessToken = formSaveDto?.AccessToken;


            //CREAT RETURN VALUE
            Submit201Output output = new Submit201Output()
            {
                RecordId = formdata.RecordId,
                RecordMatterId = formdata.RecordMatterId,
                RecordMatterItemId = formdata.RecordMatterItemId,
                SubmissionId = formdata.SubmissionId,
                requestId = -1
            };

            // SUBMIT TO GETEDGEAPI
            ASIC201Dto getEdgeInput = MapTo201(formdata);
            HttpResponseMessage response = null;
            using (var client = new HttpClient())
            {

                string FormJson = JsonConvert.SerializeObject(getEdgeInput);
                client.DefaultRequestHeaders.Add("X-Auth-Edge", _getEdgeConfig.Value.XAuthEdge);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var stringContent = new StringContent(FormJson, Encoding.UTF8, "application/json");
                response = client.PostAsync($"{_getEdgeConfig.Value.GetEdgeAPI}/201", stringContent).Result;  // Blocking call!  
                var responseresult = await response.Content.ReadAsStringAsync();

                var result = new GetRequestIdDTO();
                try
                {
                    result = JsonConvert.DeserializeObject<GetRequestIdDTO>(responseresult);
                }
                catch (Exception e)
                {
                    throw new UserFriendlyException(e.Message, responseresult);
                }

                try
                {
                    CreateOrEditAsicDto asicRequest = new CreateOrEditAsicDto()
                    {
                        HTTPRequests = $"{_getEdgeConfig.Value.GetEdgeAPI}/201/",
                        RequestMethod = AsicConsts.RequestMethod.Post,
                        Response = responseresult,
                        Data = Convert.ToString(formSaveDto.Submission),
                        RequestId = result.response != null ? Convert.ToInt32(result.response) : -1,
                        RecordId = formSaveDto.RecordId,
                        RecordMatterId = formSaveDto.RecordMatterId,
                        RecordMatterItemId = formSaveDto.RecordMatterItemId,
                        SubmissionId = formSaveDto.SubmissionId,
                        AccessToken = formSaveDto.AccessToken,
                        Status = "new"
                    };

                    // CREATE ASIC REQUEST ENTITY
                    await CreateOrEdit(asicRequest);

                    if (result.response != null)
                    {
                        await _asicDocumentAssemblyManager.HangfireCheckStatus((int)result.response, formdata);
                        output.requestId = (int)result.response;
                        return output;
                    }
                }
                catch (Exception e)
                {
                    throw new UserFriendlyException(Convert.ToString(e.Message), Convert.ToString(e.Source));
                }

            }
            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Asic_Create, AppPermissions.Pages_Asic_Edit)]
        public MemoryStream DownloadAllDocumentsByRequestId(int requestId)
        {

            Asic asicrequest = _asicRepository.FirstOrDefault(a => a.RequestId == requestId);

            if (asicrequest != null)
            {
                ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = asicrequest.RecordMatterItemId, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });
                if (ACLResult.IsAuthed)
                {
                    var rmis = _recordMatterItemRepository.GetAll().Where(j => j.RecordMatterId == asicrequest.RecordMatterId).ToList();

                    MemoryStream memoryStream = new MemoryStream();
                    using (ZipArchive zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {

                        foreach (RecordMatterItem rmi in rmis)
                        {

                            if (rmi != null && !string.IsNullOrEmpty(rmi.DocumentName) && rmi.AllowedFormats != null)
                            {

                                if (rmi.AllowedFormats.Contains("W"))
                                {
                                    var rmid = _projectsAppService.GetDocumentForDownload(rmi.Id, 1, "docx");

                                    if (rmid != null)
                                    {
                                        if (rmi.Document != null)
                                        {
                                            ZipArchiveEntry readmeEntry = zip.CreateEntry(rmid.RecordMatterItem.DocumentName);
                                            using (var zipStream = readmeEntry.Open())
                                            {
                                                zipStream.Write(rmid.RecordMatterItem.Document, 0, rmid.RecordMatterItem.Document.Length);
                                            }
                                        }
                                    }
                                }

                                if (rmi.AllowedFormats.Contains("P"))
                                {
                                    var rmid = _projectsAppService.GetDocumentForDownload(rmi.Id, 1, "pdf");
                                    if (rmid != null)
                                    {
                                        if (rmi.Document != null)
                                        {
                                            ZipArchiveEntry readmeEntry = zip.CreateEntry(rmid.RecordMatterItem.DocumentName);
                                            using (var zipStream = readmeEntry.Open())
                                            {
                                                zipStream.Write(rmid.RecordMatterItem.Document, 0, rmid.RecordMatterItem.Document.Length);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    return memoryStream;

                }
                else
                {
                    throw new Exception("ASIC Request does not exist or not Authorised");
                }
            }
            else
            {
                throw new UserFriendlyException("ASIC Request does not exist or not Authorised");
            }


        }

        [AbpAuthorize(AppPermissions.Pages_Asic_Create, AppPermissions.Pages_Asic_Edit)]
        public async Task<GetDocumentResultDto> GetAllDocumentsByRequestId(int requestId)
        {

            GetDocumentResultDto output = null;

            //role checking, asic pression
            Asic asicrequest = _asicRepository.FirstOrDefault(a => a.RequestId == requestId);

            if (asicrequest != null)
            {
                ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = asicrequest.RecordMatterId, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });


                if (!ACLResult.IsAuthed)
                {
                    throw new UserFriendlyException("ASIC Request does not exist or not Authorised");
                }

                Submission submission = _submissionRepository.GetAll()
                     .Include(i => i.RecordFk)
                     .Include(i => i.RecordMatterFk)
                     .Include(i => i.UserFk)
                     .FirstOrDefault(i => i.Id == asicrequest.SubmissionId);

                if (submission != null)
                {
                    output = new GetDocumentResultDto
                    {
                        TenantId = submission.TenantId,
                        FormId = submission.FormId,
                        RecordMatterId = submission.RecordMatterId,
                        CreationTime = submission.CreationTime,
                        LastModificationTime = submission.LastModificationTime,
                        SubmissionId = submission.Id,
                        SubmissionStatus = submission.SubmissionStatus
                    };


                    if (submission.SubmissionStatus == "Complete")
                    {
                        List<RecordMatterItem> recordMatterItems =
                            _recordMatterItemRepository.GetAll().Where(
                                i => i.SubmissionId == asicrequest.SubmissionId &&
                                i.RecordMatterId == asicrequest.RecordMatterId
                            ).OrderBy("CreationTime desc")
                            .ToList();


                        List<DocumentList201Dto> docs = new List<DocumentList201Dto>();

                        recordMatterItems.ForEach(i =>
                        {
                            DocumentList201Dto docstatus = new DocumentList201Dto();
                            // docstatus.Status = submission.SubmissionStatus;
                            //add Id?
                            docstatus.RecordmatterItemId = i.Id;
                            docstatus.DocumentName = i.DocumentName;
                            docstatus.LockOnBuild = i.LockOnBuild;
                            docstatus.DocumentTemplateId = i.DocumentTemplateId;
                            docstatus.AllowedFormats = i.AllowedFormats;

                            docs.Add(docstatus);
                        });

                        output.DocList = docs;
                    }
                }
            }
            else
            {
                throw new UserFriendlyException("ASIC Request does not exist or not Authorised");
            }

            return output;

        }

        [AbpAuthorize(AppPermissions.Pages_Asic_Create, AppPermissions.Pages_Asic_Edit)]
        public async Task<string> Check201Status(int requestId)
        {
            Asic asicrequest = _asicRepository.FirstOrDefault(a => a.RequestId == requestId);

            if (asicrequest != null)
            {
                ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = asicrequest.RecordMatterId, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });


                if (!ACLResult.IsAuthed)
                {
                    throw new UserFriendlyException("ASIC Request does not exist or not Authorised");
                }
                HttpResponseMessage response = null;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-Auth-Edge", _getEdgeConfig.Value.XAuthEdge);

                    client.DefaultRequestHeaders
                            .Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    response = client.GetAsync($"{_getEdgeConfig.Value.GetEdgeAPI}/checkStatus/" + requestId).Result;  // Blocking call!  

                    var responseresult = await response.Content.ReadAsStringAsync();

                    return responseresult;
                }
            }
            else
            {
                throw new UserFriendlyException("ASIC Request does not exist or not Authorised");
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Asic_Create, AppPermissions.Pages_Asic_Edit)]
        public async Task<byte[]> GetAsic201Documents(int requestId)
        {
            Asic asicrequest = _asicRepository.FirstOrDefault(a => a.RequestId == requestId);

            if (asicrequest != null)
            {
                ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = asicrequest.RecordMatterId, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });


                if (!ACLResult.IsAuthed)
                {
                    throw new UserFriendlyException("ASIC Request does not exist or not Authorised");
                }
                HttpResponseMessage response = null;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-Auth-Edge", _getEdgeConfig.Value.XAuthEdge);

                    client.DefaultRequestHeaders
                            .Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    response = client.GetAsync($"{_getEdgeConfig.Value.GetEdgeAPI}/201/docs/" + requestId).Result;  // Blocking call!  

                    var responseresult = await response.Content.ReadAsByteArrayAsync();

                    return responseresult;
                }
            }
            else
            {
                throw new UserFriendlyException("ASIC Request does not exist or not Authorised");
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Asic_Create, AppPermissions.Pages_Asic_Edit)]
        public async Task<byte[]> Get201Certificate(int requestId)
        {

            Asic asicrequest = _asicRepository.FirstOrDefault(a => a.RequestId == requestId);

            if (asicrequest != null)
            {
                ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = asicrequest.RecordMatterId, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });


                if (!ACLResult.IsAuthed)
                {
                    throw new UserFriendlyException("ASIC Request does not exist or not Authorised");
                }

                HttpResponseMessage response = null;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-Auth-Edge", _getEdgeConfig.Value.XAuthEdge);

                    client.DefaultRequestHeaders
                            .Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    response = client.GetAsync($"{_getEdgeConfig.Value.GetEdgeAPI}/201/" + requestId).Result;  // Blocking call!  
                    var responseresultCertificate = await response.Content.ReadAsByteArrayAsync();

                    return responseresultCertificate;
                }
            }
            else
            {
                throw new UserFriendlyException("ASIC Request does not exist or not Authorised");
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Asic_Create, AppPermissions.Pages_Asic_Edit)]
        public async Task<string> Check201Logs(int requestId)
        {
            Asic asicrequest = _asicRepository.FirstOrDefault(a => a.RequestId == requestId);

            if (asicrequest != null)
            {
                ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = asicrequest.RecordMatterId, UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });


                if (!ACLResult.IsAuthed)
                {
                    throw new UserFriendlyException("ASIC Request does not exist or not Authorised");
                }
                HttpResponseMessage response = null;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-Auth-Edge", _getEdgeConfig.Value.XAuthEdge);

                    client.DefaultRequestHeaders
                            .Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    response = client.GetAsync($"{_getEdgeConfig.Value.GetEdgeAPI}/checkLog/" + requestId).Result;  // Blocking call!  
                    var responseresult = await response.Content.ReadAsStringAsync();
                    return responseresult;
                }
            }
            else
            {
                throw new UserFriendlyException("ASIC Request does not exist or not Authorised");
            }
        }

        //public async Task<FileDto> GetAsicToExcel(GetAllAsicForExcelInput input)
        //{

        //    var filteredAsic = _asicRepository.GetAll()
        //                //.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.HTTPRequests.Contains(input.Filter) || e.RequestMethod.Contains(input.Filter) || e.Response.Contains(input.Filter))
        //                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.HTTPRequests.Contains(input.Filter) || e.Response.Contains(input.Filter))
        //                .WhereIf(!string.IsNullOrWhiteSpace(input.HTTPRequestsFilter), e => e.HTTPRequests == input.HTTPRequestsFilter)
        //                //.WhereIf(!string.IsNullOrWhiteSpace(input.RequestMethodFilter), e => e.RequestMethod == input.RequestMethodFilter)
        //                .WhereIf(!string.IsNullOrWhiteSpace(input.ResponseFilter), e => e.Response == input.ResponseFilter);

        //    var query = (from o in filteredAsic
        //                 select new GetAsicForViewDto()
        //                 {
        //                     Asic = new AsicDto
        //                     {
        //                         HTTPRequests = o.HTTPRequests,
        //                         RequestMethod = o.RequestMethod,
        //                         Response = o.Response,
        //                         Id = o.Id
        //                     }
        //                 });

        //    var asicListDtos = await query.ToListAsync();

        //    return _asicExcelExporter.ExportToFile(asicListDtos);
        //}

        private async Task CreateOrEdit(CreateOrEditAsicDto input)
        {
            try
            {
                if (input.Id == null)
                {
                    await Create(input);
                }
                else
                {
                    await Update(input);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }


        private async Task Create(CreateOrEditAsicDto input)
        {
            var asic = ObjectMapper.Map<Asic>(input);

            if (AbpSession.TenantId != null)
            {
                asic.TenantId = (int?)AbpSession.TenantId;
            }

            await _asicRepository.InsertAsync(asic);

        }


        private async Task Update(CreateOrEditAsicDto input)
        {
            var asic = await _asicRepository.FirstOrDefaultAsync((Guid)input.Id);
            ObjectMapper.Map(input, asic);
        }

        private string convertBirthDay(string birthday)
        {
            string converted_birthday = "";
            Regex regexUS = new Regex(@"^\d{2}\/\d{2}\/\d{4}$");
            Regex regexIso = new Regex(@"^\d{4}\-(0?[1-9]|1[012])\-(0?[1-9]|[12][0-9]|3[01])");
            if (regexUS.IsMatch(birthday))
            {
                string[] dates = birthday.Split('/');

                string mm = dates[0];
                string dd = dates[1];
                string yyyy = dates[2];
                return converted_birthday = yyyy + mm + dd;
            }
            else if (regexIso.IsMatch(birthday))
            {
                string datesIso = birthday.IndexOf('T') == -1 ? birthday : birthday.Substring(0, birthday.IndexOf('T'));
                string[] dates = datesIso.Split('-');

                string yyyy = dates[0];
                string mm = dates[1];
                string dd = dates[2];

                return converted_birthday = yyyy + mm + dd;
            }
            else
            {
                throw new UserFriendlyException("date format should be mm/dd/yyyy or yyyy-mm-dd");
            }
        }

        private Admin MapAdmin(Form201Dto input)
        {
            Admin admin = new Admin();

            admin.request_manual_review = MapYN(input?.Co_Manual_Review_yn?.Co_Manual_Review_yn?.ToString());
            admin.certificate_delivery_option = "PDF";//IS THIS HARDCODE??
            admin.has_asic_consent_for_name = MapYN(input?.Co_Name_Consent_yn?.Co_Name_Consent_yn?.ToString());
            if (MapYN(input?.Co_Manual_Review_yn?.Co_Manual_Review_yn?.ToString()) == "Y")
            {
                admin.text_manual_review = input?.Co_Manual_Review_Reason_txt?.ToString()?.ToUpper();
            }
            return admin;
        }

        private Applicant MapApplicant(Form201Dto input)
        {

            //we hard code the applicant has to be individual, not company
            Applicant applicantObject = new Applicant();

            //How to check the value here?????
            //if (input?.Client_Type_yn?.Client_Type_yn?.ToString() == "Individual")
            //{
            Name member_name_person = new Name();
            member_name_person.given_name1 = input?.Client_Name?.Name_First_txt?.ToString()?.ToUpper();
            if (input?.Client_Name?.Name_Middle_txt != null || input?.Client_Name?.Name_Middle_txt?.ToString() != "")
            {
                member_name_person.given_name2 = input?.Client_Name?.Name_Middle_txt?.ToString()?.ToUpper();
            }
            member_name_person.family_name = input?.Client_Name?.Name_Last_txt?.ToString()?.ToUpper();

            applicantObject.name_person = member_name_person;

            Address member_address = MapAddress(input?.Client);
            applicantObject.address = member_address;
            applicantObject.signatory_name = member_name_person;

            //signed date using south austrlia time zone
            var gmtTime = DateTimeOffset.UtcNow;
            applicantObject.date_signed = gmtTime.ToOffset(TimeSpan.Parse("10:30")).ToString("yyyyMMdd");

            applicantObject.confirm_assented_to = MapYN(Convert.ToString(input?.Co_Dec_Sig_yn));

            return applicantObject;

        }

        private List<HoldingOwner> MapIndividuals(Form201Dto input)
        {
            List<HoldingOwner> individuals = new List<HoldingOwner>();
            //nonshareholders
            if (MapYN(input?.Individuals_yn?.Individuals_yn?.ToString()) == "Y")
            {
                //SHNameRpt
                foreach (IndividualRpt persondetail in input?.Individual_rpt)
                {
                    HoldingOwner person = new HoldingOwner();
                    Name member_name_person = new Name();
                    member_name_person.given_name1 = persondetail?.Ind_Name_First_txt?.ToString()?.ToUpper();
                    if (persondetail?.Ind_Name_Middle_txt != null || persondetail?.Ind_Name_Middle_txt?.ToString() != "")
                    {
                        member_name_person.given_name2 = persondetail?.Ind_Name_Middle_txt?.ToString()?.ToUpper();
                    }
                    member_name_person.family_name = persondetail?.Ind_Name_Last_txt?.ToString()?.ToUpper();
                    person.member_name_person = member_name_person;

                    // var fAddress = ObjectMapper.Map<FormAddress>(persondetail.Ind);
                    Address member_address = MapAddress(persondetail?.Ind);

                    person.member_address = member_address;

                    person.address_overridden = "N";

                    individuals.Add(person);
                }
            }

            //director just office holder and not nonshare member
            //foreach (DirectorRpt item in OrEmptyIfNull(input?.Director_rpt))
            //{
            //    if (item?.Role_Member_yn?.Role_Member_yn == "false")
            //    {
            //        HoldingOwner holding_owner = new HoldingOwner();

            //        //personal details
            //        Name name = new Name();
            //        name.given_name1 = Convert.ToString(item?.Name_First_txt)?.ToUpper();
            //        if (item?.Name_Middle_txt != null || item?.Name_Middle_txt?.ToString() != "")
            //        {
            //            name.given_name2 = item?.Name_Middle_txt?.ToString()?.ToUpper();
            //        }
            //        name.family_name = item.Name_Last_txt?.ToString()?.ToUpper();
            //        holding_owner.member_name_person = name;

            //        //var faddress = ObjectMapper.Map<FormAddress>(item.Dir);
            //        Address addressi = MapAddress(item?.Dir);
            //        holding_owner.member_address = addressi;
            //        holding_owner.address_overridden = "N";

            //        individuals.Add(holding_owner);
            //    }
            //}
            return individuals;
        }

        private List<ShareClass> MapShareClassList(Form201Dto input)
        {
            List<ShareClass> share_class = new List<ShareClass>();

            //director
            if ((MapYN(input?.Shareholders_yn?.Shareholders_yn)) == "Y")
            {
                foreach (ShareholderRpt sharemember in input?.Shareholder_rpt)
                {
                    foreach (DirectorShareRpt d in sharemember?.Shareholder_Share_rpt)
                    {
                        if (!share_class.Exists(x => x.title == d?.Class_cho.Mtext?.ToString()?.ToUpper()))
                        {
                            ShareClass share_c = MapShareClass(d);
                            share_class.Add(share_c);
                        }
                        else
                        {
                            ShareClass share_c = share_class.Where(x => x.title == d?.Class_cho.Mtext?.ToString()?.ToUpper()).FirstOrDefault();
                            share_c.total_number_issued += Convert.ToDouble(d?.Nbr_num);
                            share_c.total_amount_paid += Convert.ToDouble(d?.Paid_num) * Convert.ToDouble(d?.Nbr_num) * 100;
                            share_c.total_amount_unpaid += Convert.ToDouble(d?.Unpaid_num) * 100 * Convert.ToDouble(d?.Nbr_num);
                        }
                    }
                }
            }
            //share holder
            foreach (DirectorRpt item in OrEmptyIfNull(input?.Director_rpt))
            {
                if (MapYN(item?.Role_Member_yn?.Role_Member_yn?.ToString()) == "Y")
                {
                    //item.Director_Share_rpt is null???????????????????
                    foreach (DirectorShareRpt d in item?.Director_Share_rpt)
                    {
                        if (!share_class.Exists(x => x.title == d?.Class_cho.Mtext?.ToString()?.ToUpper()))
                        {
                            ShareClass share_c = MapShareClass(d);
                            share_class.Add(share_c);
                        }
                        else
                        {
                            ShareClass share_c = share_class.Where(x => x.title == d?.Class_cho.Mtext?.ToString()?.ToUpper()).FirstOrDefault();
                            share_c.total_number_issued += Convert.ToDouble(d?.Nbr_num);
                            share_c.total_amount_paid += Convert.ToDouble(d?.Paid_num) * Convert.ToDouble(d?.Nbr_num)* 100;
                            share_c.total_amount_unpaid += Convert.ToDouble(d?.Unpaid_num) * 100 * Convert.ToDouble(d?.Nbr_num);
                        }
                    }
                }
            }

            return share_class.OrderBy(x => x.code).ToList();
        }

        private ShareClass MapShareClass(DirectorShareRpt d)
        {
            ShareClass share_c = new ShareClass();
            share_c.code = d?.Class_cho.value?.ToString()?.ToUpper();
            share_c.title = d?.Class_cho.Mtext?.ToString()?.ToUpper();
            share_c.total_number_issued = Convert.ToDouble(d?.Nbr_num);
            share_c.total_amount_paid = Convert.ToDouble(d?.Paid_num) * Convert.ToDouble(d?.Nbr_num) * 100;
            share_c.total_amount_unpaid = Convert.ToDouble(d?.Unpaid_num) * 100 * Convert.ToDouble(d?.Nbr_num);

            return share_c;

        }

        //can be share holder and director
        private List<Member> MapShareholder(Form201Dto input)
        {
            List<Member> members = new List<Member>();

            //share holders
            if (MapYN(input?.Shareholders_yn?.Shareholders_yn) == "Y")
            {
                foreach (ShareholderRpt sharemember in OrEmptyIfNull(input?.Shareholder_rpt))
                {
                    //what if sharemember.Shareholder_Share_rpt is null??????????
                    foreach (DirectorShareRpt d in sharemember?.Shareholder_Share_rpt)
                    {
                        Member member = new Member();
                        List<HoldingOwner> holding_owners = new List<HoldingOwner>();
                        member.share_class = d?.Class_cho.value?.ToString()?.ToUpper();
                        member.number = Convert.ToDouble(d?.Nbr_num);
                        if (Convert.ToInt32(Convert.ToDouble(d?.Unpaid_num)*100) == 0)
                        {
                            member.shares_fully_paid = "Y";
                        }
                        else
                        {
                            member.shares_fully_paid = "N";

                        }
                        member.beneficial_owner = MapYN(d?.Benef_yn?.value);
                        member.total_paid = Convert.ToDouble(d?.Paid_num) * Convert.ToDouble(d?.Nbr_num) * 100;
                        member.total_unpaid = Convert.ToDouble(d?.Unpaid_num) * 100 * Convert.ToDouble(d?.Nbr_num);

                        member.amount_paid_per_share = Convert.ToDouble(d?.Paid_num) * 100;
                        member.amount_due_per_share = Convert.ToDouble(d?.Unpaid_num) * 100;

                        //personal details
                        //individual details
                        if (MapYN(sharemember?.Type_Ind_yn?.Type_Ind_yn?.ToString()) == "Y")
                        {
                            foreach (SHNameRpt persondetail in sharemember?.SH_Name_rpt)
                            {
                                HoldingOwner person = new HoldingOwner();
                                Name member_name_person = new Name();
                                member_name_person.given_name1 = persondetail?.SH_Name_First_txt?.ToString()?.ToUpper();
                                if (persondetail?.SH_Name_Middle_txt != null || persondetail?.SH_Name_Middle_txt?.ToString() != "")
                                {
                                    member_name_person.given_name2 = persondetail?.SH_Name_Middle_txt?.ToString()?.ToUpper();
                                }
                                member_name_person.family_name = persondetail?.SH_Name_Last_txt?.ToString()?.ToUpper();
                                person.member_name_person = member_name_person;

                                //var faddress = ObjectMapper.Map<FormAddress>(persondetail.SH_Ind);
                                Address member_address = MapAddress(persondetail?.SH_Ind);

                                person.member_address = member_address;
                                person.address_overridden = "N";

                                holding_owners.Add(person);
                            }

                        }
                        else //company
                        {
                            HoldingOwner organization = new HoldingOwner();
                            organization.member_name_organisation = sharemember?.SH_Co_Name_txt?.ToString()?.ToUpper();
                            organization.member_acn_organisation = sharemember?.SH_Co_ACN_msk?.ToString()?.ToUpper();
                            organization.member_has_acn = MapYN(sharemember?.SH_Co_ACN_yn?.SH_Co_ACN_yn);
                            if (MapYN(sharemember?.SH_Co_ACN_yn?.SH_Co_ACN_yn) == "N")
                            {
                                organization.member_acn_organisation = null;
                            }
                            // var faddress = ObjectMapper.Map<FormAddress>(sharemember.SH);
                            organization.member_address = MapAddress(sharemember?.SH);
                            organization.address_overridden = "N";
                            holding_owners.Add(organization);
                        }
                        member.holding_owners = holding_owners;
                        members.Add(member);
                    }
                }
            }
            //End of share holder

            //directors
            foreach (DirectorRpt item in OrEmptyIfNull(input?.Director_rpt))
            {
                //List<HoldingOwner> holding_owners = new List<HoldingOwner>();
                if (MapYN(Convert.ToString(item?.Role_Member_yn?.Role_Member_yn)) == "Y")
                {
                    foreach (DirectorShareRpt d in item?.Director_Share_rpt)
                    {
                        Member member = new Member();
                        HoldingOwner holding_owner = new HoldingOwner();

                        member.share_class = d?.Class_cho?.value?.ToString()?.ToUpper();
                        member.number = Convert.ToDouble(d?.Nbr_num);
                        if (Convert.ToInt32(Convert.ToDouble(d?.Unpaid_num)*100) == 0)
                        {
                            member.shares_fully_paid = "Y";
                        }
                        else
                        {
                            member.shares_fully_paid = "N";
                        }
                        member.beneficial_owner = MapYN(d?.Benef_yn?.value);
                        member.total_paid = Convert.ToDouble(d?.Paid_num) * Convert.ToDouble(d?.Nbr_num) * 100;
                        member.total_unpaid = Convert.ToDouble(d?.Unpaid_num) * 100 * Convert.ToDouble(d?.Nbr_num);

                        member.amount_paid_per_share = Convert.ToDouble(d?.Paid_num )* 100;
                        member.amount_due_per_share = Convert.ToDouble(d?.Unpaid_num) * 100;

                        //personal details
                        Name name = new Name();
                        name.given_name1 = Convert.ToString(item?.Name_First_txt)?.ToUpper();
                        if (item?.Name_Middle_txt != null || item?.Name_Middle_txt?.ToString() != "")
                        {
                            name.given_name2 = item?.Name_Middle_txt?.ToString()?.ToUpper();
                        }
                        name.family_name = item?.Name_Last_txt?.ToString()?.ToUpper();
                        holding_owner.member_name_person = name;

                        //var faddress = ObjectMapper.Map<FormAddress>(item.Dir);
                        Address addressi = MapAddress(item?.Dir);
                        holding_owner.member_address = addressi;
                        holding_owner.address_overridden = "N";

                        //holding_owners.Add(holding_owner);
                        member.holding_owners = new List<HoldingOwner>();
                        member.holding_owners.Add(holding_owner);
                        members.Add(member);
                    }
                }
            }
            //End of directors

            return members.OrderBy(x => x.share_class).ToList();
        }

        //individual, shareholder, director
        private List<Officer> MapOfficer(Form201Dto input)
        {
            List<Officer> officers = new List<Officer>();

            foreach (DirectorRpt item in OrEmptyIfNull(input?.Director_rpt))
            {
                Officer officer = new Officer();
                Name name = new Name();
                name.given_name1 = Convert.ToString(item?.Name_First_txt)?.ToUpper();
                if (item?.Name_Middle_txt != null || item?.Name_Middle_txt?.ToString() != "")
                {
                    name.given_name2 = item?.Name_Middle_txt?.ToString()?.ToUpper();
                }
                name.family_name = item?.Name_Last_txt?.ToString()?.ToUpper();
                officer.name = name;

                BirthDetails birth_details = new BirthDetails();//format("YYYY-MM-DDT00:00:00Z")

                //YYYYMMDD
                //use regex to convert date
                birth_details.date = convertBirthDay(Convert.ToString(item?.DOB_dt));
                //  birth_details.date = DateTime.Parse(Convert.ToString(item?.DOB_dt)).ToString("yyyyMMdd");


                birth_details.locality = item?.Birth_City_txt?.ToString()?.ToUpper();

                birth_details.locality_qualifier = item?.Birth_State_cho?.value?.ToString()?.ToUpper();
                if (item?.Birth_Country_txt?.mtext == "AU" || item?.Birth_Country_txt?.value?.ToString()?.ToUpper() == "AUSTRALIA")
                {
                    birth_details.locality_qualifier = item?.Birth_State_cho?.value?.ToString()?.ToUpper();
                }
                else
                {
                    birth_details.locality_qualifier = item?.Birth_Country_txt?.value?.ToString()?.ToUpper();
                }
                officer.birth_details = birth_details;

                //var faddress = ObjectMapper.Map<FormAddress>(item.Dir);
                Address addressi = MapAddress(item?.Dir);
                officer.address = addressi;
                officer.address_overridden = "N";

                List<string> offices = new List<string>();
                //former name is not include in the form and the schema
                if (MapYN(item?.Role_Secretary_yn?.Role_Secretary_yn?.ToString()) == "Y")
                {
                    offices.Add("SEC");
                }
                offices.Add("DIR");

                officer.offices = offices;

                officers.Add(officer);
            }

            // Start share holder
            if (MapYN(input?.Shareholders_yn?.Shareholders_yn) == "Y")
            {
                foreach (ShareholderRpt sharemember in OrEmptyIfNull((input?.Shareholder_rpt)))
                {
                    //what if sharemember.Shareholder_Share_rpt is null??????????
                    //shareholder

                    if (MapYN(sharemember?.Type_Ind_yn?.Type_Ind_yn?.ToString()) == "Y")
                    {
                        if (MapYN(sharemember?.SH_Role_Secretary_yn?.SH_Role_Secretary_yn?.ToString()) == "Y")
                        {
                            List<string> offices = new List<string>();
                            offices.Add("SEC");

                            Officer officer = new Officer();
                            officer.offices = offices;

                            if (sharemember?.SH_Name_rpt?.Count == 1)
                            {
                                foreach (SHNameRpt persondetail in sharemember?.SH_Name_rpt)
                                {
                                    Name member_name_person = new Name();
                                    member_name_person.given_name1 = persondetail?.SH_Name_First_txt?.ToString()?.ToUpper();
                                    if (persondetail?.SH_Name_Middle_txt != null || persondetail?.SH_Name_Middle_txt?.ToString() != "")
                                    {
                                        member_name_person.given_name2 = persondetail?.SH_Name_Middle_txt?.ToString()?.ToUpper();
                                    }
                                    member_name_person.family_name = persondetail?.SH_Name_Last_txt?.ToString()?.ToUpper();
                                    officer.name = member_name_person;

                                    Address addressi = MapAddress(persondetail?.SH_Ind);
                                    officer.address = addressi;
                                }
                            }
                            else
                            {
                                throw new UserFriendlyException("join shareholder cannot be sec");
                            }


                            BirthDetails birth_details = new BirthDetails();//format("YYYY-MM-DDT00:00:00Z")

                            //YYYYMMDD
                            //birth_details.date = DateTime.Parse(Convert.ToString(sharemember?.SH_DOB_dt)).ToString("yyyyMMdd");
                            birth_details.date = convertBirthDay(Convert.ToString(sharemember?.SH_DOB_dt));
                            birth_details.locality = sharemember?.SH_Birth_City_txt?.ToString()?.ToUpper();

                            birth_details.locality_qualifier = sharemember?.SH_Birth_State_cho?.value?.ToString()?.ToUpper();

                            if (sharemember?.SH_Birth_Country_txt?.mtext == "AU" || sharemember?.SH_Birth_Country_txt?.value?.ToString()?.ToUpper() == "AUSTRALIA")
                            {
                                birth_details.locality_qualifier = sharemember?.SH_Birth_State_cho?.value?.ToString()?.ToUpper();
                            }
                            else
                            {
                                birth_details.locality_qualifier = sharemember?.SH_Birth_Country_txt?.value?.ToString()?.ToUpper();
                            }

                            officer.birth_details = birth_details;

                            officer.address_overridden = "N";
                            officers.Add(officer);
                        }

                    }

                }
            }
            //End Share holder

            //map individual
            if (MapYN(input?.Individuals_yn?.Individuals_yn?.ToString()) == "Y")
            {
                foreach (IndividualRpt persondetail in input?.Individual_rpt)
                {
                    if (MapYN(persondetail?.Ind_Role_Secretary_yn?.Ind_Role_Secretary_yn?.ToString()) == "Y")
                    {
                        List<string> offices = new List<string>();
                        offices.Add("SEC");

                        Officer officer = new Officer();
                        officer.offices = offices;

                        Name member_name_person = new Name();
                        member_name_person.given_name1 = persondetail?.Ind_Name_First_txt?.ToString()?.ToUpper();
                        if (persondetail?.Ind_Name_Middle_txt != null || persondetail?.Ind_Name_Middle_txt?.ToString() != "")
                        {
                            member_name_person.given_name2 = persondetail?.Ind_Name_Middle_txt?.ToString()?.ToUpper();
                        }
                        member_name_person.family_name = persondetail?.Ind_Name_Last_txt?.ToString()?.ToUpper();
                        officer.name = member_name_person;

                        // var fAddress = ObjectMapper.Map<FormAddress>(persondetail.Ind);
                        Address member_address = MapAddress(persondetail?.Ind);

                        officer.address = member_address;

                        officer.address_overridden = "N";

                        BirthDetails birth_details = new BirthDetails();//format("YYYY-MM-DDT00:00:00Z")

                        //YYYYMMDD
                        //  birth_details.date = DateTime.Parse(Convert.ToString(persondetail?.Ind_DOB_dt)).ToString("yyyyMMdd");
                        birth_details.date = convertBirthDay(Convert.ToString(persondetail?.Ind_DOB_dt));
                        birth_details.locality = persondetail?.Ind_Birth_City_txt?.ToString()?.ToUpper();
                        birth_details.locality_qualifier = persondetail?.Ind_Birth_State_cho?.value?.ToString()?.ToUpper();

                        if (persondetail?.Ind_Birth_Country_txt?.mtext == "AU" || persondetail?.Ind_Birth_Country_txt?.value?.ToString()?.ToUpper() == "AUSTRALIA")
                        {
                            birth_details.locality_qualifier = persondetail?.Ind_Birth_State_cho?.value?.ToString()?.ToUpper();
                        }
                        else
                        {
                            birth_details.locality_qualifier = persondetail?.Ind_Birth_Country_txt?.value?.ToString()?.ToUpper();
                        }

                        officer.birth_details = birth_details;

                        officers.Add(officer);
                    }
                }
            }
            //End map individual


            return officers;
        }

        public IEnumerable<T> OrEmptyIfNull<T>(IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        private UltimateHolding MapUltimateHolding(Form201Dto input)
        {
            UltimateHolding u = new UltimateHolding();
            u.name = input?.Co_Ult_Holding_Name_txt?.ToUpper();
            u.acn = input?.Co_Ult_Holding_ACN_msk?.ToUpper();
            u.place_incorporation = input?.Co_Ult_Holding_Place_Incorp_cho?.value?.ToUpper();//test????
            u.abn = input?.Co_Ult_Holding_ABN_msk?.ToUpper();
            return u;
        }

        private PrincipalPlace MapPrincipalPlace(Form201Dto input)
        {
            PrincipalPlace p = new PrincipalPlace();
            p.address_overridden = "N";
            //var faddress = ObjectMapper.Map<FormAddress>(input.Co);
            p.address = MapAddress(input?.Co);

            return p;
        }
        private RegisteredOffice MapRegistered_office(Form201Dto input)
        {
            RegisteredOffice ro = new RegisteredOffice();
            // var faddress = ObjectMapper.Map<FormAddress>(input.Co_Regd);

            if (MapYN(Convert.ToString(input?.Co_Regd_Addr_Same_yn?.Co_Regd_Addr_Same_yn)) == "Y")
            {
                ro.address = MapAddress(input?.Co);
            }
            else
            {
                ro.address = MapAddress(input?.Co_Regd);
            }

            ro.occupy = MapYN(input?.Co_Addr_Regd_Occ_yn?.Co_Addr_Regd_Occ_yn);
            //if occupy is N
            if (ro.occupy == "N")
            {
                ro.occupier_name = input?.Co_Addr_Regd_Occ_txt?.ToUpper();
                ro.occupant_consent = MapYN(input?.Co_Addr_Regd_Occ_Per_yn?.Co_Addr_Regd_Occ_Per_yn);//////////???
            }

            ro.address_overridden = "N";
            return ro;
        }

        private Address MapAddress(FormAddress a)
        {
            Address address = new Address();

            if (a != null)
            {
                if (a.Addr_Co_txt != null || a.Addr_Co_txt?.ToString() != "")
                {
                    address.care_of = a.Addr_Co_txt?.ToUpper();
                }
                if (a.Addr_Level_txt != null || a.Addr_Level_txt != "")
                {
                    address.line2 = a.Addr_Level_txt?.ToUpper();
                }
                address.street = a?.Addr_1_txt?.ToUpper();
                address.locality = a?.Addr_Suburb_txt?.ToUpper();
                //do we need to set to AU??
                // address.country = a?.Addr_Country_cho?.ToUpper();
                if (a?.Addr_Country_cho?.ToString()?.ToUpper() != "AUSTRALIA")
                {
                    address.country = a?.Addr_Country_cho?.ToUpper();
                }

                if (a?.Addr_Country_cho?.ToString()?.ToUpper() == "AUSTRALIA" || a?.Addr_Country_cho?.ToString()?.ToUpper() == "AU" || String.IsNullOrEmpty(a?.Addr_Country_cho))
                {
                    address.state = a?.Addr_State_txt?.ToUpper();
                    address.postcode = a?.Addr_PC_txt?.ToUpper();
                }
            }
            return address;

        }

        private List<Business> MapBusiness(Form201Dto input)
        {
            List<Business> bs = new List<Business>();
            //Business b = new Business();
            if (!String.IsNullOrEmpty(input?.Co_RBN_SA_msk))
            {
                Business b = new Business();
                b.place_registration = "SA";
                b.registration_number = input?.Co_RBN_SA_msk?.ToUpper();
                bs.Add(b);
            }
            if (!String.IsNullOrEmpty(input?.Co_RBN_ACT_msk))
            {
                Business b = new Business();
                b.place_registration = "ACT";
                b.registration_number = input?.Co_RBN_ACT_msk?.ToUpper();
                bs.Add(b);
            }
            if (!String.IsNullOrEmpty(input?.Co_RBN_NSW_msk))
            {
                Business b = new Business();
                b.place_registration = "NSW";
                b.registration_number = input?.Co_RBN_NSW_msk?.ToUpper();
                bs.Add(b);
            }
            if (!String.IsNullOrEmpty(input?.Co_RBN_NT_msk))
            {
                Business b = new Business();
                b.place_registration = "NT";
                b.registration_number = input?.Co_RBN_NT_msk?.ToUpper();
                bs.Add(b);
            }
            if (!String.IsNullOrEmpty(input?.Co_RBN_QLD_msk))
            {
                Business b = new Business();
                b.place_registration = "QLD";
                b.registration_number = input?.Co_RBN_QLD_msk?.ToUpper();
                bs.Add(b);
            }
            if (!String.IsNullOrEmpty(input?.Co_RBN_TAS_msk))
            {
                Business b = new Business();
                b.place_registration = "TAS";
                b.registration_number = input?.Co_RBN_TAS_msk?.ToUpper();
                bs.Add(b);
            }
            if (!String.IsNullOrEmpty(input?.Co_RBN_VIC_msk))
            {
                Business b = new Business();
                b.place_registration = "VIC";
                b.registration_number = input?.Co_RBN_VIC_msk?.ToUpper();
                bs.Add(b);
            }
            if (!String.IsNullOrEmpty(input?.Co_RBN_WA_msk))
            {
                Business b = new Business();
                b.place_registration = "WA";
                b.registration_number = input?.Co_RBN_WA_msk?.ToUpper();
                bs.Add(b);
            }
            return bs;
        }

        private Reservation MapReservation(Form201Dto input)
        {
            Reservation reservation = new Reservation();

            if (MapYN(input?.CO_410_Applicant_Type_yn?.CO_410_Applicant_Type_yn) == "Y")
            {
                Name name = new Name();
                name.family_name = input?.CO_410_Name_Last_txt?.ToUpper();
                name.given_name1 = input?.CO_410_Name_First_txt?.ToUpper();
                reservation.applicant_name_person = name;
            }
            else
            {
                reservation.applicant_name_organisation = input?.CO_410_Client_Name_txt?.ToUpper();
            }

            reservation.document_number = input?.CO_410_Number_txt?.ToUpper();
            return reservation;

        }

        private CompanyId MapCompanyId(Form201Dto input)
        {
            CompanyId company = new CompanyId();
            if (MapYN(input?.Co_Name_As_ACN_yn?.Co_Name_As_ACN_yn) == "N")
            {
                company.company_name = String.Concat(input?.Co_Name_txt?.ToUpper(), " ", input?.Co_Legal_Element_cho?.value?.ToUpper());
            }
            company.company_type = input?.Co_Type_cho?.ToUpper(); //hard code in Syntaq form201 "APTY"
            company.company_class = input?.Co_Class_cho?.ToUpper();
            company.company_subclass = input?.Co_Class_Sub_cho_MText?.ToUpper();
            company.acn_yesno = MapYN(input?.Co_Name_As_ACN_yn?.Co_Name_As_ACN_yn);
            company.acn_legal = input?.Co_Legal_Element_cho?.value?.ToUpper();

            //company_trpe in form is hard code always be "APTY"
            company.reserved_410 = MapYN(input?.CO_410_yn?.CO_410_yn);
            company.name_identical = (MapYN(input?.ASIC_ABN_yn?.ASIC_ABN_yn) == "Y" || MapYN(input?.Co_RBN_yn?.Co_RBN_yn) == "Y") ? "Y" : "N";
            company.jurisdiction = input?.Co_Place_Incorp_cho?.value?.ToUpper();

            //company.governed_constitution??? mapping to what if company_type is APTY???

            //form sets the value of the Registered Office Address to the same one entered above at Principal Place of Business
            //This value always hard code as "Y";
            company.residential_officeholder = "Y";
            if (MapYN(input?.ASIC_ABN_yn?.ASIC_ABN_yn?.ToString()) == "Y")
            {
                company.abn = input?.ASIC_ABN_msk?.ToUpper();
            }

            return company;
        }

        private string MapYN(string result)
        {
            if (result?.ToLower() == "false" || result?.ToLower() == "no")
            {
                return "N";
            }
            else if (result?.ToLower() == "true" || result?.ToLower() == "yes")
            {
                return "Y";
            }

            return "";

        }

    }
}