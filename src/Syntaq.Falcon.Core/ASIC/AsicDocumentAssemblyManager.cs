using System;
using System.Linq;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Timing;
using Abp.Domain.Uow;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.Records;
using System.Collections.Generic;
using Abp.Threading;
using System.Diagnostics;
using Syntaq.Falcon.Configuration;
using Abp.Auditing;
using Abp.UI;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.ASIC;
using Abp.Net.Mail;
using Syntaq.Falcon.Apps;
using Syntaq.Falcon.Url;
using Abp.MultiTenancy;
using Syntaq.Falcon.ASIC.Dtos;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Abp.Runtime.Session;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

using Microsoft.AspNetCore.Http;
using Syntaq.Falcon.Forms;
using Microsoft.Extensions.Options;
using Syntaq.Falcon.Web;
using Syntaq.Falcon.Authorization;
using Abp.Authorization;
using System.Text.Json;
using System.Net.Mail;
using Hangfire;
using Syntaq.Falcon.Utility;

namespace Syntaq.Falcon.ASIC
{
    [Audited]
    public class AsicDocumentAssemblyManager: FalconDomainServiceBase
    {
        private readonly IRepository<Asic, Guid> _asicRepository;
        private readonly IEmailSender _emailSender;
        private readonly IAppsAppService _appsAppService;
        private readonly IFormsAppService _formsAppService;
        private readonly RecordManager _recordManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<User, long> _userRepository;
        private readonly IWebUrlService _webUrlService;
        private readonly ITenantCache _tenantCache;
        private readonly ACLManager _ACLManager;
        private readonly IRepository<RecordMatter, Guid> _recordMatterRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IOptions<GetEdgeConfig> _getEdgeConfig;
        private readonly IAbpSession _AbpSession;
        private readonly UserEmailer _userEmailer;

        public AsicDocumentAssemblyManager(
           ACLManager aclManager,
           IAppsAppService appsAppService,
           IFormsAppService formsAppService,
           UserManager userManager, 
           IUnitOfWorkManager unitOfWorkManager,
           RecordManager recordManager,
           IRepository<Asic, Guid> asicRepository, 
           IEmailSender emailSender,
           IWebUrlService webUrlService,
           IRepository<RecordMatter, Guid> recordMatterRepository,
           IHttpContextAccessor httpContextAccessor,
           ITenantCache tenantCache, 
           IOptions<GetEdgeConfig> getEdgeConfig,
           IAbpSession AbpSession,
           UserEmailer userEmailer,
           IRepository<User, long> userRepository)
        {
            _recordMatterRepository = recordMatterRepository;
            _ACLManager = aclManager;
            _appsAppService = appsAppService;
            _formsAppService = formsAppService;
            _asicRepository = asicRepository;
            _emailSender = emailSender;
            _recordManager = recordManager;
            _unitOfWorkManager = unitOfWorkManager;
            _userRepository = userRepository;
            _webUrlService = webUrlService;
            _tenantCache = tenantCache;
            _httpContextAccessor = httpContextAccessor;
            _getEdgeConfig = getEdgeConfig;
            _AbpSession = AbpSession;
            _userEmailer = userEmailer;
        }

       
        [UnitOfWork]
        //have to internal private in the deplyment
        public async Task HangfireCheckStatus(int requestId, Form201Dto formdata)
        {
            try
            {
                Guid formId = formdata.FormId;

                var responseresult = await CheckStatus(requestId);
                var result = JsonConvert.DeserializeObject<CheckStatusDto>(responseresult);

                if (result.error.Count == 0)
                {
                    var status = result.response.SelectToken("status")?.ToString();
                    switch (status)
                    {
                        case "finished":
                            await NotifyUser(requestId, true, formdata);
                            break;
                        case "new":
                            BackgroundJob.Schedule<AsicDocumentAssemblyManager>(x => x.HangfireCheckStatus(requestId, formdata), TimeSpan.FromMinutes(3));
                            break;
                        case "transmission ok":
                            BackgroundJob.Schedule<AsicDocumentAssemblyManager>(x => x.HangfireCheckStatus(requestId, formdata), TimeSpan.FromMinutes(3));
                            break;
                        case "validation ok":
                            BackgroundJob.Schedule<AsicDocumentAssemblyManager>(x => x.HangfireCheckStatus(requestId, formdata), TimeSpan.FromMinutes(3));
                            break;
                        case "validation failed":
                            await NotifyUser(requestId,false, formdata);
                            break;
                        case "retry 00x":
                            BackgroundJob.Schedule<AsicDocumentAssemblyManager>(x => x.HangfireCheckStatus(requestId, formdata), TimeSpan.FromMinutes(3));
                            break;
                        case "rejected":
                            await NotifyUser(requestId, false, formdata);
                            break;
                        case "sent":
                            BackgroundJob.Schedule<AsicDocumentAssemblyManager>(x => x.HangfireCheckStatus(requestId, formdata), TimeSpan.FromMinutes(3));
                            break;
                        case "manual review":
                            NotifyManualReview(requestId, formdata);
                            BackgroundJob.Schedule<AsicDocumentAssemblyManager>(x => x.HangfireCheckStatus(requestId, formdata), TimeSpan.FromMinutes(15)); 
                            break;
                        default:
                            BackgroundJob.Schedule<AsicDocumentAssemblyManager>
                                (x => x.HangfireCheckStatus(requestId, formdata), TimeSpan.FromMinutes(5));
                            break;
                    }
                    Logger.Debug(requestId + "ASIC!!  CHECK STATUS" + status);
                }
                else
                {
                    throw new UserFriendlyException("Check Status Error" + responseresult);
                }
            }
            catch (Exception e)
            {
                Logger.Error(requestId + "ASIC ERROR!!  CHECK STATUS"+ e.Source + e.TargetSite + e.Message + e.InnerException  + e.StackTrace);
                throw new UserFriendlyException(e.Source + e.TargetSite + e.Message + e.InnerException + e.StackTrace);
            }
        }

        public void NotifyManualReview(int requestId, Form201Dto formdata)
        {
            //try { 
           
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete, AbpDataFilters.MayHaveTenant))
                {
                   
                    string httpRequest = $"{_getEdgeConfig.Value.GetEdgeAPI}/201/";

                    var asicRequest = _asicRepository.GetAll()
                        .Where(x => x.RequestId == requestId && x.HTTPRequests == httpRequest)
                        .OrderByDescending(x => x.CreationTime)
                        .FirstOrDefault();

                    string errorEmail = formdata?.Send_eml;

                    if (asicRequest.ManualReview == false)
                    {
                        Logger.Debug(requestId + "ASIC START NotifyManualReview");
                        string bodymessage = _userEmailer.SendMannualReviewEmail(asicRequest.TenantId, formdata.Client_Name.Name_First_txt);
                        try
                        {
                            _AbpSession.Use(asicRequest.TenantId, null);

                            MailMessage mail = new MailMessage
                            {
                                Subject = $"Your application to register {formdata.Co_Name_txt} has been delayed",
                                Body = bodymessage,
                                IsBodyHtml = true
                            };

                            formdata?.Send_eml?.Split(';')?.ToList().ForEach(j =>
                            {
                                if (j != "")
                                {
                                    j = j.Trim();
                                    if (RegexUtility.IsValidEmail(j))
                                    {
                                        mail.To.Add(new MailAddress(j));
                                    }
                                }
                            });
                            _emailSender.Send(mail);
                        }
                        catch (Exception e)
                        {
                            Logger.Error(requestId + "ASIC ERROR!!  Mannual Review" + e.Message + e.Source + e.StackTrace);
                        }

                        asicRequest.Status = "manual review";
                        asicRequest.ManualReview = true;
                        _asicRepository.Update(asicRequest);
                        CurrentUnitOfWork.SaveChanges();
                        Logger.Debug(requestId + "ASIC FINISH NotifyManualReview");
                    }
                 }
                unitOfWork.Complete();
            }
        }

        public async Task NotifyUser(int requestId, bool? success, Form201Dto formdata)
        {
            Guid formId = formdata.FormId;

            int? tenantId = null;
            bool asictriggered = false;
            string userName;
            string checkstatus = "finish";

            //if fail, use check status to know "validation fail" or "reject"
            if(success == false)
            {
                var check_status_result = await CheckStatus(requestId);
                var result = JsonConvert.DeserializeObject<CheckStatusDto>(check_status_result);
                checkstatus = result.response.SelectToken("status")?.ToString();
            }

            var responseresult = Task.Run(async() => await CheckLogs(requestId)).Result;

            using var jDoc = System.Text.Json.JsonDocument.Parse(responseresult);
            responseresult = System.Text.Json.JsonSerializer.Serialize(jDoc, new JsonSerializerOptions { WriteIndented = true });
            

            //update asic datatable
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete, AbpDataFilters.MayHaveTenant))
                {
                    var asicRequest = await _asicRepository
                        .FirstOrDefaultAsync(x => x.RequestId == requestId && x.HTTPRequests == $"{_getEdgeConfig.Value.GetEdgeAPI}/201/");
                    tenantId = asicRequest.TenantId;
                    asictriggered = asicRequest.Triggered;

                    //If success, update the asic data field
                    if(success == true)
                    {
                        var result = JsonConvert.DeserializeObject<CheckLogDto>(responseresult);
                        var data = JsonConvert.DeserializeObject<JObject>(asicRequest.Data);
                        data["Co_ACN_msk"] = result?.response[0]?.communication?.values?.ZCO?.acn;
                        asicRequest.Data = JsonConvert.SerializeObject(data);
                    }
                    asicRequest.Triggered = true;
                    asicRequest.Status = checkstatus;
                    await _asicRepository.UpdateAsync(asicRequest);
                    var user = await _userRepository.FirstOrDefaultAsync(i => i.Id == asicRequest.CreatorUserId && i.TenantId == tenantId);
                    userName = user?.UserName;
                    CurrentUnitOfWork.SaveChanges();
                }
                unitOfWork.Complete();
            }

            if (!asictriggered)
            {
                if (success == false)
                { 
                   
                    try
                    {
                        Logger.Debug(requestId + "ASIC START Notify Fail MAIL");
                        _AbpSession.Use(tenantId, null);

                        //send email to developer
                        if (!String.IsNullOrEmpty(formdata?.Error_eml))     //send error json response email
                        {
                            MailMessage mail1 = new MailMessage //send to errorEmail
                            {
                                Subject = $"Fail from ASIC for {userName}",
                                Body = responseresult,
                                IsBodyHtml = false
                            };
                            formdata?.Error_eml?.Split(";")?.ToList().ForEach(j =>
                            {
                                if (j != "")
                                {
                                    j = j.Trim();
                                    if (RegexUtility.IsValidEmail(j))
                                    {
                                        mail1.To.Add(new MailAddress(j));
                                    }
                                }
                            });

                            _emailSender.Send(mail1);
                        }

                        //send email to user
                        string bodymessage = _userEmailer.SendRejectEmail(tenantId, formdata.Client_Name.Name_First_txt);
                        MailMessage mail = new MailMessage
                        {
                            Subject = $"Your application to register {formdata.Co_Name_txt} has been delayed",
                            Body = bodymessage,
                            IsBodyHtml = true
                        };

                        formdata?.Send_eml?.Split(';')?.ToList().ForEach(j =>
                        {
                            if (j != "")
                            {
                                j = j.Trim();
                                if (RegexUtility.IsValidEmail(j))
                                {
                                    mail.To.Add(new MailAddress(j));
                                }
                            }
                        });
                        _emailSender.Send(mail);
                        Logger.Debug(requestId + "ASIC FINISH Notify Fail MAIL");

                    }
                    catch (Exception e)
                    {
                        Logger.Error(requestId+ " ASIC ERROR!!" + e.Message + e.Source + e.StackTrace);
                        // Carry on SMTP Error will be logged to Audit Log
                    }
                    //if fail, save record matter                    
                    string saveformdto = await SaveForm(formdata.FormId, formdata.RecordId, formdata.RecordMatterId, formdata.RecordMatterItemId, formdata.SubmissionId);
                    Logger.Debug(requestId + " ASIC Finish save form " + saveformdto);                                      
                }
                else
                {
                    //if success, trigger assembly process
                    await AssembleDocuments(requestId, formId, formdata);    
                    Logger.Debug(requestId + " ASIC AssembleDocuments ");
                }
            }

           
        }

        [UnitOfWork]
        public async Task AssembleDocuments(int requestId, Guid formId, Form201Dto formdata)
        {
                var responseresult = Task.Run(async () => await CheckLogs(requestId)).Result;
                var result = JsonConvert.DeserializeObject<CheckLogDto>(responseresult);

                //true trigger app 
                using (var unitOfWork = _unitOfWorkManager.Begin()) {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete, AbpDataFilters.MayHaveTenant))
                {
                    //should be replace to record
                    var rm = _recordMatterRepository.GetAll().Where(e => e.Id == formdata.RecordMatterId).FirstOrDefault();

                   if (rm == null || rm.Data ==null) Logger.Error(requestId + " ASIC Record Matter Id is null");

                    if (rm != null && rm.Data != null)
                        {
                            if (result?.response[0]?.communication?.values?.ZCO?.acn == null)
                            {
                                Logger.Debug(requestId + " ASIC ACO ACN is null!! ");
                                //do nothing, because this is a test request;
                                //await NotifyUser(requestId, false, formdata);
                             }
                            else
                            {
                                    var data = JsonConvert.DeserializeObject<JObject>(rm.Data);
                                    Logger.Debug(requestId + " ASIC ASSEMBLE START!! ");
                                    //running records data check newest data of application
                                    data["RecordMatterId"] = formdata.RecordMatterId;
                                    data["RecordMatterItemId"] = formdata.RecordMatterItemId;
                                    data["Co_ACN_msk"] = result?.response[0]?.communication?.values?.ZCO?.acn;
                                    data["Co_Est_dt"] = DateTime.ParseExact(result?.response[0]?.communication?.values?.ZCO?.date_of_registration, "yyyyMMdd", null).ToString("yyyy-MM-dd");
                                    data["ASICStatus_scr"] = "success";
                         
                                    string accesstoken = rm.AccessToken;

                                    var formData = JsonConvert.SerializeObject(data);

                                    var responseresultCertificate = await Get201Certificate(requestId);
                                    var rmi = await _recordManager.CreateAndOrFetchRecordMatterItem(new RecordMatterItem()
                                    {
                                        Id = Guid.NewGuid(),
                                        AllowedFormats = "P",
                                        AllowHtmlAssignees = "[]",
                                        AllowPdfAssignees = "[]",
                                        AllowWordAssignees = "[]",
                                        Document = responseresultCertificate,
                                        DocumentName = $"Registration Certificate for {Convert.ToString(data["Co_Name_txt"])} {Convert.ToString(data["Co_Legal_Element_cho"]["value"])}.pdf ", //AB#9681 Show Comany name in certificate
                                        TenantId = _AbpSession.TenantId,
                                        RecordMatterId = rm.Id,
                                        FormId = null,
                                        DocumentTemplateId = $"{_getEdgeConfig.Value.GetEdgeAPI}/201/" + requestId,
                                        GroupId = new Guid(),
                                        UserId = null,
                                        SubmissionId = formdata.SubmissionId,
                                        LockOnBuild = true,
                                        Order = 3,
                                        HasDocument = true
                                    });

                                    // Run Form not App after the registrtion certificate has been inserted
                                var status =  await RunForm(formId, accesstoken, Convert.ToString(formdata.RecordMatterId), formData);
                                if (status == true) Logger.Debug(requestId + " ASIC ASSEMBLE FINISH!! " + status);
                                else Logger.Error(requestId + " ASIC ASSEMBLE FAIL!! " + status);
                            }

                        }
                        unitOfWork.Complete();
                    }
                }

        }
        //end of assemble documents

        public async Task <string> SaveForm(Guid formId, Guid recordId, Guid rmid,Guid rmiId, Guid SubmissionId)
        {
            string data="";
            string accessToken = "";
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete, AbpDataFilters.MayHaveTenant))
                {
                    //should be replace to record
                    var rm = await _recordMatterRepository.FirstOrDefaultAsync(e => e.Id == rmid);
                   
                    if (rm != null && rm.Data != null)
                    {
                        var rmdata = JsonConvert.DeserializeObject<JObject>(rm.Data);
                        rmdata["ASICStatus_scr"] = "error"; // will data in record be updated at the same time?
                        data = JsonConvert.SerializeObject(rmdata);
                        accessToken = rm.AccessToken;
                    }

                    var jsonstr = $@"{{
                        ""AnonAuthToken"": ""{accessToken}"",
                        ""AccessToken"": ""{accessToken}"",
                        ""Id"": ""{formId}"",
                        ""RecordId"": ""{recordId}"",
                        ""RecordMatterId"": ""{rmid}"",
                        ""RecordMatterItemId"": ""{rmiId}"",
                        ""Submission"": {data},
                    }}";
                    string result = await _formsAppService.Save(jsonstr);
                  
                    unitOfWork.Complete();
                    return result;
                }
            }
        }

        public async Task<bool> RunForm(Guid formId, string accessToken, string rmid, string data)
        {

            var jsonstr = $@"{{
              ""AnonAuthToken"": ""{accessToken}"",
              ""id"": ""{formId}"",
              ""RecordMatterId"": ""{rmid}"",
              ""submission"": {{
                ""data"": {data},
                ""state"": ""submitted"",
                ""saved"": false
              }},
            }}";

            JObject input = JObject.Parse(jsonstr);
            bool result = await _formsAppService.Run(jsonstr);
            return result;
        }

        public async Task<string> CheckStatus(int requestId)
        {
            HttpResponseMessage response = null;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Auth-Edge", _getEdgeConfig.Value.XAuthEdge);

                client.DefaultRequestHeaders
                        .Accept
                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                response = client.GetAsync($"{_getEdgeConfig.Value.GetEdgeAPI}/checkStatus/" + requestId).Result;  // Blocking call!  

                var responseresult = await response.Content.ReadAsStringAsync();

                //data is JSON PAYLOAD, need to be clean up in the future, the RECORD should store the certain DATA send to ASIC
               
                return responseresult;
            }
        }

        public async Task<byte[]> Get201Certificate(int requestId)
        {
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


        private async Task<string> CheckLogs(int requestId)
        {
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
    }
}
