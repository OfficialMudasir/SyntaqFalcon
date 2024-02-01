using Syntaq.Falcon.Records;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Forms;

using Syntaq.Falcon.Projects;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Syntaq.Falcon.Records.Exporting;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Dto;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using static Syntaq.Falcon.Projects.ProjectConsts;
using System.Net.Mail;
using Abp.Net.Mail;
using Microsoft.AspNetCore.Http;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.AccessControlList;
using Abp.UI;
using System.Collections.Immutable;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authorization;
using static Syntaq.Falcon.Records.RecordMatterContributorConsts;
using NUglify.Helpers;
using Abp.Domain.Uow;
using System.IO;
using Syntaq.Falcon.Utility;
using System.Text;
using Syntaq.Falcon.Net.Emailing;
using Abp.MultiTenancy;
using Syntaq.Falcon.Storage;
using Syntaq.Falcon.MultiTenancy;
using Newtonsoft.Json.Linq;
using Syntaq.Falcon.Settings.Dtos;
using Microsoft.Extensions.Options;
using Syntaq.Falcon.Web;
using System.Net;
using Abp.Configuration;
using Abp.Runtime.Security;
using Microsoft.Extensions.Configuration;
//using Twilio.TwiML.Voice;

namespace Syntaq.Falcon.Records
{

    public class RecordMatterContributorsAppService : FalconAppServiceBase, IRecordMatterContributorsAppService
    {

        private readonly ACLManager _ACLManager;
        private readonly IRepository<RecordMatterContributor, Guid> _recordMatterContributorRepository;
        private readonly IRecordMatterContributorsExcelExporter _recordMatterContributorsExcelExporter;
        private readonly IRepository<Project, Guid> _lookup_projectRepository;
        private readonly IRepository<Record, Guid> _lookup_recordRepository;
        private readonly IRepository<RecordMatter, Guid> _lookup_recordMatterRepository;
        private readonly IRepository<RecordMatterItem, Guid> _lookup_recordMatterItemRepository;

        private readonly IRepository<ProjectRelease, Guid> _lookup_ProjectReleaseRepository;

        private readonly IRepository<User, long> _lookup_userRepository;
        private readonly IRepository<Form, Guid> _lookup_formRepository;

        private readonly IHttpContextAccessor _contextAccessor;

        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplateProvider _emailTemplateProvider;
        private readonly ITenantCache _tenantCache;

        private readonly IFormsAppService _formAppService;

        private readonly IOptions<AppConfig> _AppConfig;

        private readonly IConfiguration _configuration;
        private readonly int _jwtExpiry = 365;
        private readonly IOptions<JSONWebToken> _JSONWebToken;

        public RecordMatterContributorsAppService(
            ACLManager aclManager,
            IEmailSender emailSender,
            IEmailTemplateProvider emailTemplateProvider,
            IRepository<RecordMatterContributor, Guid> recordMatterContributorRepository,
            IRecordMatterContributorsExcelExporter recordMatterContributorsExcelExporter,
            IRepository<Project, Guid> lookup_projectRepository,
            IRepository<Record, Guid> lookup_recordRepository,
            IRepository<RecordMatter, Guid> lookup_recordMatterRepository,
            IRepository<RecordMatterItem, Guid> lookup_recordMatterItemRepository,
            IRepository<ProjectRelease, Guid> lookup_projectreleaserepository,
            IRepository<User, long> lookup_userRepository,
            IRepository<Form, Guid> lookup_formRepository,
            IHttpContextAccessor contextAccessor,
            ITenantCache tenantCache,
            IFormsAppService formAppService,
            IOptions<AppConfig> appConfig,
            IOptions<JSONWebToken> JSONWebToken,
            IConfiguration configuration
        )
        {
            _ACLManager = aclManager;
            _contextAccessor = contextAccessor;
            _recordMatterContributorRepository = recordMatterContributorRepository;
            _recordMatterContributorsExcelExporter = recordMatterContributorsExcelExporter;
            _lookup_projectRepository = lookup_projectRepository;
            _lookup_recordRepository = lookup_recordRepository;
            _lookup_recordMatterRepository = lookup_recordMatterRepository;
            _lookup_recordMatterItemRepository = lookup_recordMatterItemRepository;
            _lookup_userRepository = lookup_userRepository;
            _lookup_formRepository = lookup_formRepository;
            _emailSender = emailSender;
            _emailTemplateProvider = emailTemplateProvider;
            _tenantCache = tenantCache;
            _formAppService = formAppService;

            _lookup_ProjectReleaseRepository = lookup_projectreleaserepository;

            _AppConfig = appConfig;

            _configuration = configuration;
            //_jwtExpiry = _configuration.GetValue<int>("JSONWebToken:Expiry", 365);
            _jwtExpiry = JSONWebToken.Value.Expiry;
            _JSONWebToken = JSONWebToken;
        }

        public async Task<GetRecordMatterContributorForViewDto> GetRecordMatterContributorForView(Guid id)
        {
            var recordMatterContributor = await _recordMatterContributorRepository.GetAsync(id);

            if (recordMatterContributor != null)
            {
                ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
                {
                    Action = "View",
                    EntityId = (Guid)recordMatterContributor.RecordMatterId,
                    UserId = AbpSession.UserId,
                    AccessToken = string.Empty,
                    TenantId = AbpSession.TenantId
                });


                if (ACLResult.IsAuthed)
                {
                    var output = new GetRecordMatterContributorForViewDto { RecordMatterContributor = ObjectMapper.Map<RecordMatterContributorDto>(recordMatterContributor) };

                    if (output.RecordMatterContributor.RecordMatterId != null)
                    {
                        var _lookupRecordMatter = await _lookup_recordMatterRepository.FirstOrDefaultAsync((Guid)output.RecordMatterContributor.RecordMatterId);
                        output.RecordMatterRecordMatterName = _lookupRecordMatter?.RecordMatterName?.ToString();
                    }

                    if (output.RecordMatterContributor.UserId != null)
                    {
                        var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.RecordMatterContributor.UserId);
                        output.UserName = _lookupUser?.Name?.ToString();
                    }

                    if (output.RecordMatterContributor.FormId != null)
                    {
                        var _lookupForm = await _lookup_formRepository.FirstOrDefaultAsync((Guid)output.RecordMatterContributor.FormId);
                        output.FormName = _lookupForm?.Name?.ToString();
                    }

                    return output;
                }
                else
                {
                    throw new UserFriendlyException("Not Authorised");
                }

            }

            throw new UserFriendlyException("Not Authorised");
        }

        [AbpAuthorize(AppPermissions.Pages_RecordMatterContributors_Edit)]
        public async Task<GetRecordMatterContributorForEditOutput> GetRecordMatterContributorForEdit(EntityDto<Guid> input, EntityDto<Guid> recordmatterId, EntityDto<Guid> formId)
        {
            var recordMatterContributor = await _recordMatterContributorRepository.FirstOrDefaultAsync(input.Id);

            if (recordMatterContributor != null)
            {
                ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
                {
                    Action = "Edit",
                    EntityId = (Guid)recordMatterContributor.RecordMatterId,
                    UserId = AbpSession.UserId,
                    AccessToken = string.Empty,
                    TenantId = AbpSession.TenantId
                });


                if (ACLResult.IsAuthed)
                {

                    var output = new GetRecordMatterContributorForEditOutput { RecordMatterContributor = ObjectMapper.Map<CreateOrEditRecordMatterContributorDto>(recordMatterContributor) };

                    var _lookupRecordMatter = await _lookup_recordMatterRepository.FirstOrDefaultAsync((Guid)recordmatterId.Id);
                    output.RecordMatterRecordMatterName = _lookupRecordMatter?.RecordMatterName?.ToString();

                    if (output.RecordMatterContributor.UserId != null)
                    {
                        var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.RecordMatterContributor.UserId);
                        output.UserName = _lookupUser?.Name?.ToString();
                    }

                    var _lookupForm = await _lookup_formRepository.FirstOrDefaultAsync((Guid)formId.Id);
                    output.FormName = _lookupForm?.Name?.ToString();
                    output.FormSchema = _lookupForm?.Schema?.ToString();
                    output.FormPages = recordMatterContributor.FormPages;
                    return output;

                }
                else
                {
                    throw new UserFriendlyException("Not Authorised");
                }

            }

            throw new UserFriendlyException("Not Authorised");


        }

        public async Task CreateOrEdit(CreateOrEditRecordMatterContributorDto input)
        {

            if( _recordMatterContributorRepository.GetAll().Any(e => e.RecordMatterId == input.RecordMatterId && e.StepStatus == 0 && e.Enabled))
            {
                throw new UserFriendlyException("Cannot invite a Contributor while waiting other contributions.");
            }

            // Compile FormSchema
            Newtonsoft.Json.Linq.JObject form = Newtonsoft.Json.Linq.JObject.Parse(input.FormSchema);
            Newtonsoft.Json.Linq.JArray formComponents = new Newtonsoft.Json.Linq.JArray();

            var pages = input.FormPages.Split("&");

            if (string.IsNullOrEmpty(input.FormPages) || pages.Length == 0)
            {
                throw new UserFriendlyException("There are no Pages to share for the Contributor");
            }

            //page is each shared componet
            foreach (var pagekey in pages)
            {
                if (!pagekey.Contains("="))
                {
                    throw new UserFriendlyException("Selected Page error for Contributor");
                }

                var key = pagekey.Split("=")[1];
                Newtonsoft.Json.Linq.JToken page = form.SelectToken("$.components[?(@.key == '" + key + "')]");
                if (page != null)
                    formComponents.Add(page);
            }

            form.SelectToken("components").Replace(formComponents);
            input.FormSchema = form.ToString();
            input.AccessToken = JwtSecurityTokenProvider.GenerateToken(DateTime.Now.ToString(), _jwtExpiry);
            input.Enabled = true;

            if (input.Id == null)
            {
                input.AccessToken = JwtSecurityTokenProvider.GenerateToken(DateTime.Now.ToString(), _jwtExpiry);
                input.Id = Guid.NewGuid();

                // await Create(input);

                var recordMatterContributor = ObjectMapper.Map<RecordMatterContributor>(input);
                if (AbpSession.TenantId != null)
                {
                    recordMatterContributor.TenantId = (int?)AbpSession.TenantId;
                }

                await _recordMatterContributorRepository.InsertAsync(recordMatterContributor);
                CurrentUnitOfWork.SaveChanges();

            }
            else
            {

                var recordMatterContributor = await _recordMatterContributorRepository.GetAsync((Guid)input.Id);
                if (recordMatterContributor != null)
                {
                    ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
                    {
                        Action = "Edit",
                        EntityId = (Guid)recordMatterContributor.RecordMatterId,
                        UserId = AbpSession.UserId,
                        AccessToken = string.Empty,
                        TenantId = AbpSession.TenantId
                    });

                    if (ACLResult.IsAuthed)
                    {
                        await Update(input);
                        CurrentUnitOfWork.SaveChanges();
                    }
                    else
                    {
                        throw new UserFriendlyException("Not Authorised");
                    }

                    ApplyInput applyinput = new ApplyInput()
                    {
                        AccessToken = recordMatterContributor.AccessToken,
                        Status = RecordMatterContributorStatus.Awaiting
                    };
                    ApplyProjectStatus(applyinput);

                }
            }

            await SendInvite(input);

        }

        [AbpAuthorize(AppPermissions.Pages_RecordMatterContributors_Edit)]
        public string GetAccessURL(CreateOrEditRecordMatterContributorDto input)
        {
            var recordMatterContributor = _recordMatterContributorRepository.GetAllIncluding(e => e.UserFk).FirstOrDefault(e => e.Id == input.Id);
            string href = ""; 
            if (recordMatterContributor != null && recordMatterContributor.Enabled == true)
            {
                ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
                {
                    Action = "Edit",
                    EntityId = (Guid)recordMatterContributor.RecordMatterId,
                    UserId = AbpSession.UserId,
                    AccessToken = string.Empty, 
                    TenantId = AbpSession.TenantId
                });


                if (ACLResult.IsAuthed)
                {
                    //var view = input.UserId.HasValue ? "loadauth" : "loadanon";
                    var view = "loadanon"; // always usin anon
                    var hostvalue = _AppConfig.Value.WebSiteRootAddress;
                    if (hostvalue.EndsWith("/")) hostvalue = hostvalue.Remove(hostvalue.Length - 1);

                    // var tenancyNameOrNull = AbpSession.TenantId.HasValue ? _tenantCache.GetOrNull((int)AbpSession.TenantId)?.TenancyName : null;
                     var tenancyNameOrNull = AbpSession.TenantId.HasValue ? TenantManager.GetTenantName((int)AbpSession.TenantId).Result : "host";
 
                    hostvalue = hostvalue.Replace("{TENANT_NAME}", tenancyNameOrNull);
                    href = $"{hostvalue}/Falcon/forms/{view}?AccessToken={recordMatterContributor.AccessToken}&RecordMatterId={recordMatterContributor.RecordMatterId}&RecordMatterItemId={input.RecordMatterItemId}&tenant={tenancyNameOrNull}";
                }
                else
                {
                    throw new UserFriendlyException("Not Authorised");
                }
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }
            return href;
        }

        [AbpAuthorize(AppPermissions.Pages_RecordMatterContributors_Edit)]
        public async Task SendInvite(CreateOrEditRecordMatterContributorDto input)
        {

            var recordMatterContributor = _recordMatterContributorRepository.GetAllIncluding(e => e.UserFk).FirstOrDefault(e => e.Id == input.Id);

            if (recordMatterContributor != null)
            {
                ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
                {
                    Action = "Edit",
                    EntityId = (Guid)recordMatterContributor.RecordMatterId,
                    UserId = AbpSession.UserId,
                    AccessToken = string.Empty,
                    TenantId = AbpSession.TenantId
                });


                if (ACLResult.IsAuthed)
                {
                    string authorname = string.Empty;
                    string projectname = string.Empty;
                    string recordmattername = string.Empty;
                    string status = string.Empty;
                    string projectdescription = string.Empty;
                    string projecttype = string.Empty;
                    string projectsteprole = recordMatterContributor.StepRole.ToString().ToLower();
                    string authorusername = string.Empty;

                    recordMatterContributor.Status = RecordMatterContributorStatus.Awaiting;
                    recordMatterContributor.Enabled = true;

                    // If this is a resend get the record matteritmid 
                    var recordmatter = _lookup_recordMatterRepository.GetAllIncluding(e => e.User).Where(e => e.Id == recordMatterContributor.RecordMatterId).FirstOrDefault();
                    if (recordmatter != null)
                    {
                        input.RecordMatterId = recordmatter.Id;
                        recordmattername = recordmatter.RecordMatterName;
                        //status = recordmatter.Status;

                        var record = _lookup_recordRepository.GetAll().Where(e => e.Id == recordmatter.RecordId).FirstOrDefault();
 
                        var project = _lookup_projectRepository.GetAll().Where(p => p.RecordId == recordmatter.RecordId).FirstOrDefault();
                        projectdescription = project?.Description;
                        projectname = project?.Name;

                        CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant);

                        var rpId = _lookup_ProjectReleaseRepository.GetAll().Where(p => p.Id == project.ReleaseId).FirstOrDefault()?.ProjectId;
                        if (rpId != null){
                            projecttype = _lookup_projectRepository.GetAll().Where(p => p.Id == rpId).FirstOrDefault()?.Name;
                        }

                        CurrentUnitOfWork.EnableFilter(AbpDataFilters.MayHaveTenant);

                        var author = _lookup_userRepository.GetAll().Where(e => e.Id == recordmatter.CreatorUserId).FirstOrDefault();
                        authorname = author?.Name + ' ' + author?.Surname;
                        authorusername = author?.UserName;
                    }

                    //the modified email template
                    // If this is a resend get the record matteritmid 
                    if (!input.RecordMatterItemId.HasValue)
                    {
                        var recordmatteritem = _lookup_recordMatterItemRepository.GetAll().Where(e => e.RecordMatterId == recordMatterContributor.RecordMatterId).FirstOrDefault();
                        if (recordmatteritem != null)
                        {
                            input.RecordMatterItemId = recordmatteritem.Id;
                        }
                    }

                    //var view = input.UserId.HasValue ? "loadauth" : "loadanon";
                    var view = "loadanon"; // always usin anon

                    var hostvalue = _AppConfig.Value.WebSiteRootAddress;
                    //var tenancyNameOrNull = AbpSession.TenantId.HasValue ? _tenantCache.GetOrNull((int)AbpSession.TenantId)?.TenancyName : null;
                    var tenancyNameOrNull = AbpSession.TenantId.HasValue ? TenantManager.GetTenantName((int)AbpSession.TenantId).Result : "host";

                    //var hostvalue = this._contextAccessor.HttpContext.Request.Host.Value;
                    if (hostvalue.EndsWith("/")) hostvalue = hostvalue.Remove(hostvalue.Length - 1);
                    hostvalue = hostvalue.Replace("{TENANT_NAME}", tenancyNameOrNull);
                    var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(AbpSession.TenantId));
                    emailTemplate.Replace("{EMAIL_SUB_TITLE}", "");

                    var mailto = $"<a href='mailto:{recordmatter.User.EmailAddress}' subject='Review shared in error'>{recordmatter.User.EmailAddress}</a>";

                    var href = $"{hostvalue}/Falcon/forms/{view}?AccessToken={recordMatterContributor.AccessToken}&RecordMatterId={recordMatterContributor.RecordMatterId}&RecordMatterItemId={input.RecordMatterItemId}&tenant={tenancyNameOrNull}";

                    recordmattername = string.IsNullOrEmpty(recordmattername) ? "Open" : recordmattername;

                    var firstname = recordMatterContributor.Name;

                    if (recordMatterContributor.UserFk != null)
                    {
                        firstname = recordMatterContributor.UserFk.Name;
                    };


                    var body = $"<p>Hi {firstname},</p>";
                    body += $"<p>{authorname} has invited you to {projectsteprole} the following document in the {tenancyNameOrNull} Document Builder platform.</p>";
                    

                    body += $"<p>To access the shared document, follow the link below:</p>";
                    body += $"<p><a href = '{href}'>{projecttype}: {projectname}</a></p>";

                    if (!string.IsNullOrEmpty(input.Message))
                        body += $"<p>{input.Message}</p>";

                    body += $"<p>Note, this link will expire in 20 days. To refresh the link, email {mailto}</p>";

                    if (input.UserId.HasValue) //to define invite guest or not
                    {
                        body += $"<p>Alternatively, you can access the shared document by logging directly into your Document Builder account.</p>";
                    }

                    body += $"<p>This is an automated message from {tenancyNameOrNull}, please do not reply.</p>";

                    emailTemplate.Replace("{TENANT_NAME}", tenancyNameOrNull);
                    emailTemplate.Replace("{EMAIL_BODY}", body);
                 
                    MailMessage mail = new MailMessage
                    {
                        // Invitation to (<review> or <approve>) <Project Type>: < Project Name > from < Author Firstname > < Author Surname >
                        Subject = $"Invitation to {projectsteprole} {projecttype}: {projectname} from {authorname}",
                        Body = emailTemplate.ToString(),
                        IsBodyHtml = true
                    };

                    // Attach Documents
                    if (recordmatter != null)
                    {

                        // Add documents if approver and documents exist
                        if (recordMatterContributor.StepRole == Projects.ProjectConsts.ProjectStepRole.Approve)
                        {
                            var draft = false;
                            var documents = _lookup_recordMatterItemRepository.GetAll().Where(e => e.RecordMatterId == recordMatterContributor.RecordMatterId).ToList();

                            // Check to see if all contributions are complete for step
                            var recordMatter = _lookup_recordMatterRepository.GetAll().FirstOrDefault(e => e.Id == recordMatterContributor.RecordMatterId);
                            var contributors = _recordMatterContributorRepository.GetAll().Where(e => e.RecordMatterId == recordMatterContributor.RecordMatterId);

                            var endorsecanceled = contributors.Where(e => e.StepRole == ProjectStepRole.Review && e.Status == RecordMatterContributorStatus.Canceled).Count();
                            var endorsecomplete = contributors.Where(e => e.StepRole == ProjectStepRole.Review && e.Status == RecordMatterContributorStatus.Approved).Count();
                            var endorsenew = contributors.Where(e => e.StepRole == ProjectStepRole.Review && e.Status == RecordMatterContributorStatus.Awaiting).Count();
                            var endorserejected = contributors.Where(e => e.StepRole == ProjectStepRole.Review && e.Status == RecordMatterContributorStatus.Rejected).Count();
                            var endorsetotal = endorsecanceled + endorsecomplete + endorsenew + endorserejected;

                            var approvedcanceled = contributors.Where(e => e.StepRole == ProjectStepRole.Approve && e.Status == RecordMatterContributorStatus.Canceled).Count();
                            var approvedcomplete = contributors.Where(e => e.StepRole == ProjectStepRole.Approve && e.Status == RecordMatterContributorStatus.Approved).Count();
                            var approvednew = contributors.Where(e => e.StepRole == ProjectStepRole.Approve && e.Status == RecordMatterContributorStatus.Awaiting).Count();
                            var approvedrejected = contributors.Where(e => e.StepRole == ProjectStepRole.Approve && e.Status == RecordMatterContributorStatus.Rejected).Count();
                            var approvedtotal = approvedcanceled + approvedcomplete + approvednew + approvedrejected;

                            RecordMatterConsts.RecordMatterStatus recordMatterStatus = RecordMatterConsts.RecordMatterStatus.Draft;

                            if (endorsetotal > 0)
                            {
                                if (endorserejected + endorsecanceled + endorsenew > 0)
                                {
                                    recordMatterStatus = RecordMatterConsts.RecordMatterStatus.Share;
                                }
                            }

                            if (approvedtotal > 0)
                            {
                                if (approvedrejected + approvedcanceled + approvednew > 0)
                                {
                                    recordMatterStatus = RecordMatterConsts.RecordMatterStatus.Share;
                                }
                                if (approvedcomplete == approvedtotal && endorsecomplete == endorsetotal)
                                {
                                    recordMatterStatus = RecordMatterConsts.RecordMatterStatus.Final;
                                }
                            }

                            draft = recordMatterStatus == RecordMatterConsts.RecordMatterStatus.Final ? false : true;
                            documents.ForEach(e => {

                                Byte[] bydoc = e.Document;

                                if (bydoc != null)
                                {
                                    if (e.AllowedFormats.ToLower().Contains("p"))
                                    {
                                        Byte[] bydpdf = AsposeUtility.BytesToPdf(bydoc, draft);
                                        Attachment att = new Attachment(new MemoryStream(bydpdf), e.DocumentName + ".pdf");
                                        mail.Attachments.Add(att);
                                    }

                                    if (e.AllowedFormats.ToLower().Contains("w"))
                                    {
                                        Byte[] bytdocx = AsposeUtility.BytesToWord(bydoc, draft);
                                        Attachment att = new Attachment(new MemoryStream(bytdocx), e.DocumentName + ".docx");
                                        mail.Attachments.Add(att);
                                    }
                                }

                            });
                        }

                    }

                    mail.To.Add(new MailAddress(recordMatterContributor.Email));
                             
                    if (!string.IsNullOrEmpty(recordMatterContributor.EmailCC))
                    {
                        mail.CC.Add(new MailAddress(recordMatterContributor.EmailCC));
                    }

                    if (!string.IsNullOrEmpty(recordMatterContributor.EmailCC))
                    {
                        mail.Bcc.Add(new MailAddress(recordMatterContributor.EmailBCC));
                    }
 
                    var emailfrom = SettingManager.GetSettingValue("Abp.Net.Mail.DefaultFromAddress");
                    mail.From = new MailAddress(emailfrom);
                    //_emailSender.SendAsync(mail);

                    var smtpPassword = await SettingManager.GetSettingValueAsync(EmailSettingNames.Smtp.Password);

                    // Set up the SMTP client
                    //SmtpClient client = new SmtpClient(SettingManager.GetSettingValue(EmailSettingNames.Smtp.Host), Convert.ToInt16(SettingManager.GetSettingValue(EmailSettingNames.Smtp.Port)));
                    //client.UseDefaultCredentials = false;
                    //client.Credentials = new NetworkCredential(SettingManager.GetSettingValue(EmailSettingNames.Smtp.UserName), SimpleStringCipher.Instance.Decrypt(smtpPassword));
                    //client.EnableSsl = true;

                    //client.Send(mail);

                    await _emailSender.SendAsync(mail, true);


                }
                else
                {
                    throw new UserFriendlyException("Not Authorised");
                }

            }
        }


        [AbpAuthorize(AppPermissions.Pages_RecordMatterContributors_Create)]
        protected virtual async Task Create(CreateOrEditRecordMatterContributorDto input)
        {
            var recordMatterContributor = ObjectMapper.Map<RecordMatterContributor>(input);


            if (AbpSession.TenantId != null)
            {
                recordMatterContributor.TenantId = (int?)AbpSession.TenantId;
            }


            await _recordMatterContributorRepository.InsertAsync(recordMatterContributor);
            CurrentUnitOfWork.SaveChanges();
        }

        [AbpAuthorize(AppPermissions.Pages_RecordMatterContributors_Edit)]
        protected virtual async Task Update(CreateOrEditRecordMatterContributorDto input)
        {


            var recordMatterContributor = await _recordMatterContributorRepository.FirstOrDefaultAsync((Guid)input.Id);
            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
            {
                Action = "Edit",
                EntityId = (Guid)recordMatterContributor.RecordMatterId,
                UserId = AbpSession.UserId,
                AccessToken = string.Empty,
                TenantId = AbpSession.TenantId
            });

            if (!ACLResult.IsAuthed)
            {
                throw new UserFriendlyException("Not Authorised");
            }

            ObjectMapper.Map(input, recordMatterContributor);

            // On adding Contributors for first time set in progress
            var record = await _lookup_recordMatterRepository.FirstOrDefaultAsync((Guid)recordMatterContributor.RecordMatterId);
            if (record != null)
            {
                if (record.Status == RecordMatterConsts.RecordMatterStatus.New)
                {
                    record.Status = RecordMatterConsts.RecordMatterStatus.Draft;
                }

                // Set Statuses 
                // If New set record = in progress, recordmatter = Draft
                var project = _lookup_projectRepository.GetAll().Where(e => e.RecordId == (Guid)record.RecordId).FirstOrDefault();
                if (project != null)
                {
                    switch (project.Status)
                    {
                        case ProjectStatus.New:
                            project.Status = ProjectStatus.InProgress;
                            break;
                    }
                }

            }


        }

        [AbpAuthorize(AppPermissions.Pages_RecordMatterContributors_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {

            var recordMatterContributor = _recordMatterContributorRepository.GetAll().FirstOrDefault(e => e.Id == input.Id);
            if (recordMatterContributor != null)
            {
                ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
                {
                    Action = "Edit",
                    EntityId = (Guid)recordMatterContributor.RecordMatterId,
                    UserId = AbpSession.UserId,
                    AccessToken = string.Empty
                });


                if (ACLResult.IsAuthed)
                {
                    await _recordMatterContributorRepository.DeleteAsync(input.Id);
                }
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

        [AbpAuthorize(AppPermissions.Pages_RecordMatterContributors_Edit)]
        public async Task Enable(EntityDto<Guid> input)
        {

            var recordMatterContributor = _recordMatterContributorRepository.GetAll().FirstOrDefault(e => e.Id == input.Id);
            if (recordMatterContributor != null)
            {
                ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
                {
                    Action = "Edit",
                    EntityId = (Guid)recordMatterContributor.RecordMatterId,
                    UserId = AbpSession.UserId,
                    AccessToken = string.Empty,
                    TenantId = AbpSession.TenantId
                });


                if (ACLResult.IsAuthed)
                {
                    recordMatterContributor.Enabled = true;
                    recordMatterContributor.Status = RecordMatterContributorStatus.Awaiting;
                }
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }
        }


        [AbpAuthorize(AppPermissions.Pages_RecordMatterContributors_Edit)]
        public async Task Disable(EntityDto<Guid> input)
        {

            var recordMatterContributor = _recordMatterContributorRepository.GetAll().FirstOrDefault(e => e.Id == input.Id);
            if (recordMatterContributor != null)
            {
                ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto()
                {
                    Action = "Edit",
                    EntityId = (Guid)recordMatterContributor.RecordMatterId,
                    UserId = AbpSession.UserId,
                    AccessToken = string.Empty,
                    TenantId = AbpSession.TenantId
                });


                if (ACLResult.IsAuthed)
                {
                    recordMatterContributor.Enabled = false;
                }
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }
        }

        [AllowAnonymous]
        public async Task Apply(ApplyInput input)
        {
            ApplyProjectStatus(input);
            SendUpdate(input);
        }

        private void ApplyProjectStatus(ApplyInput input)
        {

            // Can be anonymous as do not know the tenant
            CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant);

            var recordMatterContributor = _recordMatterContributorRepository.GetAll().FirstOrDefault(e => e.AccessToken == input.AccessToken);
            if (recordMatterContributor != null)
            {
                recordMatterContributor.Status = input.Status;
                recordMatterContributor.Enabled = false;
                CurrentUnitOfWork.SaveChanges();

                //Set the Step Status
                // Check to see if all contributions are complete for step
                // Inprogress = any contributor not competed
                var recordMatter = _lookup_recordMatterRepository.GetAll().FirstOrDefault(e => e.Id == recordMatterContributor.RecordMatterId);
                var contributors = _recordMatterContributorRepository.GetAll().Where(e => e.RecordMatterId == recordMatterContributor.RecordMatterId);

                var reviewcanceled = contributors.Where(e => e.StepRole == ProjectStepRole.Review && e.Status == RecordMatterContributorStatus.Canceled).Count();
                var reviewcomplete = contributors.Where(e => e.StepRole == ProjectStepRole.Review && e.Status == RecordMatterContributorStatus.Approved).Count();
                var reviewnew = contributors.Where(e => e.StepRole == ProjectStepRole.Review && e.Status == RecordMatterContributorStatus.Awaiting).Count();
                var reviewrejected = contributors.Where(e => e.StepRole == ProjectStepRole.Review && e.Status == RecordMatterContributorStatus.Rejected).Count();
                var reviewtotal = reviewcanceled + reviewcomplete + reviewnew + reviewrejected;

                var approvedcanceled = contributors.Where(e => e.StepRole == ProjectStepRole.Approve && e.Status == RecordMatterContributorStatus.Canceled).Count();
                var approvedcomplete = contributors.Where(e => e.StepRole == ProjectStepRole.Approve && e.Status == RecordMatterContributorStatus.Approved).Count();
                var approvednew = contributors.Where(e => e.StepRole == ProjectStepRole.Approve && e.Status == RecordMatterContributorStatus.Awaiting).Count();
                var approvedrejected = contributors.Where(e => e.StepRole == ProjectStepRole.Approve && e.Status == RecordMatterContributorStatus.Rejected).Count();
                var approvedtotal = approvedcanceled + approvedcomplete + approvednew + approvedrejected;

                RecordMatterConsts.RecordMatterStatus recordMatterStatus = RecordMatterConsts.RecordMatterStatus.Draft;

                //if (reviewtotal  > 0)
                //{
                //    //if (reviewrejected + reviewcanceled + reviewnew   > 0)
                //    //{
                //    //    recordMatterStatus = RecordMatterConsts.RecordMatterStatus.Publish;
                //    //}
                //    if (reviewcomplete == reviewtotal   && approvedtotal == 0)
                //    {
                //        recordMatterStatus = RecordMatterConsts.RecordMatterStatus.Final;
                //    }
                //}

                if (approvedtotal > 0)
                {
                    //if(approvedrejected + approvedcanceled + approvednew > 0)
                    //{
                    //    recordMatterStatus = RecordMatterConsts.RecordMatterStatus.Publish;
                    //}
                    if (approvedcomplete == approvedtotal)
                    {
                        recordMatterStatus = RecordMatterConsts.RecordMatterStatus.Final;
                        recordMatter.Status = recordMatterStatus;
                    }
                }

                //recordMatter.Status = recordMatterStatus;
                CurrentUnitOfWork.SaveChanges();

                if (recordMatter != null)
                {
                    // Check to see if all Steps are Final
                    bool projectcompleted = !_lookup_recordMatterRepository.GetAll().Any(e => e.RecordId == recordMatter.RecordId && e.Status != RecordMatterConsts.RecordMatterStatus.Final);

                    ProjectStatus projectStatus = projectcompleted ? ProjectStatus.Completed : ProjectStatus.InProgress;
                    var project = _lookup_projectRepository.GetAll().FirstOrDefault(e => e.RecordId == recordMatter.RecordId);
                    if (project != null)
                    {
                        project.Status = projectStatus;
                        CurrentUnitOfWork.SaveChanges();
                    }
                }

                // Check if all Approvers have approved then apply updates to Record and RecordMatter



            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }


        [AllowAnonymous]
        public async Task UpdateComments(UpdateCommentsInput input)
        {

            // Can be anonymous as do not know the tenant
            CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant);

            var recordMatterContributor = _recordMatterContributorRepository.GetAll().FirstOrDefault(e => e.AccessToken == input.AccessToken);
            if (recordMatterContributor != null)
            {
                recordMatterContributor.Comments = input.Comments;
            }

        }

        private void SendUpdate(ApplyInput input)
        {

            CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant);

            var recordMatterContributor = _recordMatterContributorRepository.GetAllIncluding(e => e.RecordMatterFk).FirstOrDefault(e => e.AccessToken == input.AccessToken);
            if (recordMatterContributor != null)
            {

                AbpSession.Use(recordMatterContributor.TenantId, null);
                CurrentUnitOfWork.SetTenantId(recordMatterContributor.TenantId);

                var project = _lookup_projectRepository.GetAll().FirstOrDefault(e => e.RecordId == recordMatterContributor.RecordMatterFk.RecordId);

                string authorname = string.Empty;
                string authoremail = string.Empty;
                string projectname = string.Empty;
                string recordmattername = string.Empty;
                string status = string.Empty;
                string projecttype = string.Empty;

                string projectsteprole = recordMatterContributor.StepRole == ProjectStepRole.Review ? "reviewing" : "approving";

                // If this is a resend get the record matteritmid 
                var recordmatter = _lookup_recordMatterRepository.GetAll().Where(e => e.Id == recordMatterContributor.RecordMatterId).FirstOrDefault();
                if (recordmatter != null)
                {
                    recordMatterContributor.RecordMatterId = recordmatter.Id;
                    recordmattername = recordmatter.RecordMatterName;
                    //status = recordmatter.Status;

                    var record = _lookup_recordRepository.GetAll().Where(e => e.Id == recordmatter.RecordId).FirstOrDefault();

                    var projecttemplate = _lookup_projectRepository.GetAll().Where(e => e.Id == project.ProjectTemplateId).FirstOrDefault();
                    projecttype = projecttemplate?.Name;
                    projectname = project.Name;

                    var author = _lookup_userRepository.GetAll().Where(e => e.Id == recordmatter.CreatorUserId).FirstOrDefault();
                    authorname = author?.Name;
                    authoremail = author?.EmailAddress;

                    CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant);

                    var rpId = _lookup_ProjectReleaseRepository.GetAll().Where(p => p.Id == project.ReleaseId).FirstOrDefault()?.ProjectId;
                    if (rpId != null)
                    {
                        projecttype = _lookup_projectRepository.GetAll().Where(p => p.Id == rpId).FirstOrDefault()?.Name;
                    }

                    CurrentUnitOfWork.EnableFilter(AbpDataFilters.MayHaveTenant);

                }

                var hostvalue = _AppConfig.Value.WebSiteRootAddress;
                if (hostvalue.EndsWith("/")) hostvalue = hostvalue.Remove(hostvalue.Length - 1);
                //var hostvalue = this._contextAccessor.HttpContext.Request.Host.Value;

                var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(AbpSession.TenantId));
                emailTemplate.Replace("{EMAIL_SUB_TITLE}", "");

                //if (recordMatterContributor.UserFk == null)
                //{
                //    emailTemplate.Replace("{EMAIL_TITLE}", "Hi " + authorname);
                //}
                //else
                //{
                //    emailTemplate.Replace("{EMAIL_TITLE}", "Hi " + authorname);
                //}

                //add review or approve type of record matter contributior

                //var tenancyNameOrNull = AbpSession.TenantId.HasValue ? _tenantCache.GetOrNull((int)AbpSession.TenantId)?.TenancyName : null;
                var tenancyNameOrNull = AbpSession.TenantId.HasValue ? TenantManager.GetTenantName((int)AbpSession.TenantId).Result : "host";
                var href = $"{hostvalue}/Falcon/Projects/ViewProject/{project.Id}";

                //when contrubtor log in as auth
                if (tenancyNameOrNull == null)
                {
                    tenancyNameOrNull = project.TenantId.HasValue ? _tenantCache.GetOrNull((int)project.TenantId)?.TenancyName : null;
                }

                var body = $"<p>Hi {authorname},</p>";
                body += $"{recordMatterContributor.Name}, who was {projectsteprole} <a href='{href}'>{projecttype}: {project.Name}</a> has now returned the document to you.";

                body += $"<p>You can click on the link to view the document or you can open it up via your dashboard.</p>";

                body += $"<p>This is an automated message from {tenancyNameOrNull} please do not reply.</p>";

                //if (!string.IsNullOrEmpty(input.Message))
                //    body += $"<p>{input.Message}</p>";



                emailTemplate.Replace("{TENANT_NAME}", tenancyNameOrNull);
                emailTemplate.Replace("{EMAIL_BODY}", body);
                //< img alt = "Logo" src = "/TenantCustomization/GetLogo?TenantId=@AbpSession.TenantId" height = "38" class="kt-header__brand-logo-default">
                // var path = abp.appPath;
                // emailTemplate.Replace("{EMAIL_LOGO_URL}", "/TenantCustomization/GetLogo?TenantId=@AbpSession.TenantId");


                ////get logo
                //if (AbpSession.TenantId.HasValue)
                //{
                //    var tenant =  _tenantManager.FindByIdAsync((int)AbpSession.TenantId);
                //    //_tenantCache.GetOrNull((int)AbpSession.TenantId)
                //    //     _tenantCache.GetOrNull((int)AbpSession.TenantId) ?
                //    var logoObject =  await _binaryObjectManager.GetOrNullAsync(tenant.LogoId.Value);
                //    if (logoObject != null)
                //    {
                //        //File(logoObject.Bytes, tenant.LogoFileType);
                //        emailTemplate.Replace("{EMAIL_LOGO_URL}", "data:" + tenant.LogoFileType + ";base64," + Convert.ToBase64String(logoObject.Bytes));
                //    }
                //}
                ////end of get logo

                MailMessage mail = new MailMessage
                {
                    Subject = $"{recordMatterContributor.Name} has reviewed the {projecttype}: {project.Name}",
                    Body = emailTemplate.ToString(),
                    IsBodyHtml = true
                };

                mail.To.Add(new MailAddress(authoremail));

                if (!string.IsNullOrEmpty(recordMatterContributor.EmailCC))
                {
                    mail.CC.Add(new MailAddress(recordMatterContributor.EmailCC));
                }

                if (!string.IsNullOrEmpty(recordMatterContributor.EmailCC))
                {
                    mail.Bcc.Add(new MailAddress(recordMatterContributor.EmailBCC));
                }

                var emailfrom = SettingManager.GetSettingValue("Abp.Net.Mail.DefaultFromAddress");
                mail.From = new MailAddress(emailfrom);
                _emailSender.Send(mail);

            }
        }

 

        [AbpAuthorize(AppPermissions.Pages_RecordMatterContributors_Edit)]
        public async Task<PagedResultDto<RecordMatterContributorUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
        {
            input.Filter = input.Filter?.Trim();

            var query = _lookup_userRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name != null && e.Name.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var userList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<RecordMatterContributorUserLookupTableDto>();
            foreach (var user in userList)
            {
                lookupTableDtoList.Add(new RecordMatterContributorUserLookupTableDto
                {
                    Id = user.Id,
                    DisplayName = user.Name?.ToString(),
                    Surname = user.Surname?.ToString(),
                    Email = user.EmailAddress?.ToString(),
                    Entity = user.Entity?.ToString()
                });
            }

            return new PagedResultDto<RecordMatterContributorUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }
 


    }

}