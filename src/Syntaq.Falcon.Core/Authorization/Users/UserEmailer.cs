using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Localization;
using Abp.Net.Mail;
using Syntaq.Falcon.Chat;
using Syntaq.Falcon.Editions;
using Syntaq.Falcon.Localization;
using Syntaq.Falcon.MultiTenancy;
using System.Net.Mail;
using System.Web;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Syntaq.Falcon.Net.Emailing;
using Abp.Auditing;
using Abp.Authorization.Users;

namespace Syntaq.Falcon.Authorization.Users
{
    /// <summary>
    /// Used to send email to users.
    /// </summary>
    public class UserEmailer : FalconServiceBase, IUserEmailer, ITransientDependency
    {
        private readonly IEmailTemplateProvider _emailTemplateProvider;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly ICurrentUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ISettingManager _settingManager;
        private readonly EditionManager _editionManager;
        private readonly UserManager _userManager;
        private readonly IAbpSession _AbpSession;
        //STQ MODIFIED
        private readonly IRepository<User, long> _userRepository;

        // used for styling action links on email messages.
        private string _emailButtonStyle =
            "padding-left: 30px; padding-right: 30px; padding-top: 12px; padding-bottom: 12px; color: #ffffff; background-color: #00bb77; font-size: 14pt; text-decoration: none;";

        private string _emailButtonColor = "#00bb77";

        public UserEmailer(
            IEmailTemplateProvider emailTemplateProvider,
            IEmailSender emailSender,
            IRepository<Tenant> tenantRepository,
            ICurrentUnitOfWorkProvider unitOfWorkProvider,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            EditionManager editionManager,
            UserManager userManager,
            IAbpSession AbpSession,
            //STQ MODIFIED
            IRepository<User, long> userRepository)
        {
            _emailTemplateProvider = emailTemplateProvider;
            _emailSender = emailSender;
            _tenantRepository = tenantRepository;
            _unitOfWorkProvider = unitOfWorkProvider;
            _unitOfWorkManager = unitOfWorkManager;
            _settingManager = settingManager;
            _editionManager = editionManager;
            _userManager = userManager;
            _AbpSession = AbpSession;
            //STQ MODIFIED
            _userRepository = userRepository;
        }

        /// <summary>
        /// Send email activation link to user's email address.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Email activation link</param>
        /// <param name="plainPassword">
        /// Can be set to user's plain password to include it in the email.
        /// </param>
        [UnitOfWork]
        public virtual async Task SendEmailActivationLinkAsync(User user, string link, string plainPassword = null)
        {
            if (user.EmailConfirmationCode.IsNullOrEmpty())
            {
                throw new Exception("EmailConfirmationCode should be set in order to send email activation link.");
            }

            link = link.Replace("{userId}", user.Id.ToString());
            link = link.Replace("{confirmationCode}", Uri.EscapeDataString(user.EmailConfirmationCode));

            if (user.TenantId.HasValue)
            {
                link = link.Replace("{tenantId}", user.TenantId.ToString());
            }

            link = EncryptQueryParameters(link);

            var tenancyName = GetTenancyNameOrNull(user.TenantId);
            var emailTemplate = GetTitleAndSubTitle(user.TenantId, string.Format(L("EmailActivation_Title"), tenancyName), string.Format(L("EmailActivation_SubTitle"), tenancyName));
            emailTemplate = emailTemplate.Replace("{TENANT_NAME}", tenancyName);

            var mailMessage = new StringBuilder();

            mailMessage.AppendLine("<b>" + L("Name") + "</b>: " + user.Name + "<br />");
            mailMessage.AppendLine("<b>" + L("UserName") + "</b>: " + user.UserName + "<br />");

            //private StringBuilder GetTitleAndSubTitle(int? tenantId, string title, string subTitle)
            //{
            //    var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(tenantId));
            //    emailTemplate.Replace("{EMAIL_TITLE}", title);
            //    emailTemplate.Replace("{EMAIL_SUB_TITLE}", subTitle);

            //    return emailTemplate;
            //}


            //if (!tenancyName.IsNullOrEmpty())
            //{
            //    mailMessage.AppendLine("<b>" + L("TenancyName") + "</b>: " + tenancyName + "<br />");
            //}

            //mailMessage.AppendLine("<b>" + L("UserName") + "</b>: " + user.UserName + "<br />");
            // mailMessage.AppendLine("<b>" + user.UserName + "<b />");
            // DONT SEMD PASS WORD IN EMAIL
            //if (!plainPassword.IsNullOrEmpty())
            //{
            //    mailMessage.AppendLine("<b>" + L("Password") + "</b>: " + plainPassword + "<br />");
            //}

            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine(L("EmailActivation_ClickTheLinkBelowToVerifyYourEmail") + "<br /><br />");
            mailMessage.AppendLine("<a class=\"button\" style=\"" + _emailButtonStyle + "\" bg-color=\"" + _emailButtonColor + "\" href=\"" + link + "\">" + L("ClickButtonVerifyEmail") + "</a>");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine("<span style=\"font-size: 9pt;\">" + L("EmailMessage_CopyTheLinkBelowToYourBrowser") + "</span><br />");
            mailMessage.AppendLine("<span style=\"font-size: 8pt;\">" + link + "</span>");

            mailMessage = mailMessage.Replace("{TENANT_NAME}", tenancyName);

            await ReplaceBodyAndSend(user.EmailAddress, string.Format(L("EmailActivation_Subject"), tenancyName), emailTemplate, mailMessage);
        }

        /// <summary>
        /// Sends a password reset link to user's email.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Reset link</param>
        public async Task SendPasswordResetLinkAsync(User user, string link = null)
        {
            if (user.PasswordResetCode.IsNullOrEmpty())
            {
                throw new Exception("PasswordResetCode should be set in order to send password reset link.");
            }

            var tenancyName = GetTenancyNameOrNull(user.TenantId);
            //STQ MODIFIED
            var emailTemplate = GetTitleAndSubTitle(user.TenantId, string.Format(L("PasswordResetEmail_Title"), tenancyName), string.Format(L("PasswordResetEmail_SubTitle"), tenancyName));
            var mailMessage = new StringBuilder();

            //mailMessage.AppendLine("<b>" + L("NameSurname") + "</b>: " + user.Name + " " + user.Surname + "<br />");

            //if (!tenancyName.IsNullOrEmpty())
            //{
            //    mailMessage.AppendLine("<b>" + L("TenancyName") + "</b>: " + tenancyName + "<br />");
            //}

            //mailMessage.AppendLine("<b>" + L("UserName") + "</b>: " + user.UserName + "<br />");
            //mailMessage.AppendLine("<b>" + L("ResetCode") + "</b>: " + user.PasswordResetCode + "<br />");

            mailMessage.AppendLine("<b>" + L("PasswordEmailResetMessage") + "</b>");
            // STQ MODIFIED END

            if (!link.IsNullOrEmpty())
            {
                link = link.Replace("{userId}", user.Id.ToString());
                link = link.Replace("{resetCode}", Uri.EscapeDataString(user.PasswordResetCode));

                if (user.TenantId.HasValue)
                {
                    link = link.Replace("{tenantId}", user.TenantId.ToString());
                }

                link = EncryptQueryParameters(link);

                mailMessage.AppendLine("<br />");
                mailMessage.AppendLine(L("PasswordResetEmail_ClickTheLinkBelowToResetYourPassword") + "<br /><br />");
                mailMessage.AppendLine("<a style=\"" + _emailButtonStyle + "\" bg-color=\"" + _emailButtonColor +
                                       "\" href=\"" + link + "\">" + L("Reset") + "</a>");
                mailMessage.AppendLine("<br />");
                mailMessage.AppendLine("<br />");
                mailMessage.AppendLine("<br />");
                mailMessage.AppendLine("<span style=\"font-size: 9pt;\">" +
                                       L("EmailMessage_CopyTheLinkBelowToYourBrowser") + "</span><br />");
                mailMessage.AppendLine("<span style=\"font-size: 8pt;\">" + link + "</span>");
            }

            // STQ MODIFIED
            emailTemplate = emailTemplate.Replace("{TENANT_NAME}", tenancyName);
            mailMessage = mailMessage.Replace("{TENANT_NAME}", tenancyName);

            await ReplaceBodyAndSend(user.EmailAddress, string.Format(L("PasswordResetEmail_Subject"), tenancyName), emailTemplate, mailMessage);
        }

        public async Task TryToSendChatMessageMail(User user, string senderUsername, string senderTenancyName,
            ChatMessage chatMessage)
        {
            try
            {
                var emailTemplate = GetTitleAndSubTitle(user.TenantId, L("NewChatMessageEmail_Title"),
                    L("NewChatMessageEmail_SubTitle"));
                var mailMessage = new StringBuilder();

                mailMessage.AppendLine("<b>" + L("Sender") + "</b>: " + senderTenancyName + "/" + senderUsername +
                                       "<br />");
                mailMessage.AppendLine("<b>" + L("Time") + "</b>: " +
                                       chatMessage.CreationTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss") +
                                       " UTC<br />");
                mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + chatMessage.Message + "<br />");
                mailMessage.AppendLine("<br />");

                await ReplaceBodyAndSend(user.EmailAddress, L("NewChatMessageEmail_Subject"), emailTemplate,
                    mailMessage);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        public async Task TryToSendSubscriptionExpireEmail(int tenantId, DateTime utcNow)
        {
            try
            {
                using (_unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        var tenantAdmin = await _userManager.GetAdminAsync();
                        if (tenantAdmin == null || string.IsNullOrEmpty(tenantAdmin.EmailAddress))
                        {
                            return;
                        }

                        var hostAdminLanguage = await _settingManager.GetSettingValueForUserAsync(
                            LocalizationSettingNames.DefaultLanguage, tenantAdmin.TenantId, tenantAdmin.Id);
                        var culture = CultureHelper.GetCultureInfoByChecking(hostAdminLanguage);
                        var emailTemplate = GetTitleAndSubTitle(tenantId, L("SubscriptionExpire_Title"),
                            L("SubscriptionExpire_SubTitle"));
                        var mailMessage = new StringBuilder();

                        mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + L("SubscriptionExpire_Email_Body",
                            culture, utcNow.ToString("yyyy-MM-dd") + " UTC") + "<br />");
                        mailMessage.AppendLine("<br />");

                        await ReplaceBodyAndSend(tenantAdmin.EmailAddress, L("SubscriptionExpire_Email_Subject"),
                            emailTemplate, mailMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        public async Task TryToSendSubscriptionAssignedToAnotherEmail(int tenantId, DateTime utcNow,
            int expiringEditionId)
        {
            try
            {
                using (_unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        var tenantAdmin = await _userManager.GetAdminAsync();
                        if (tenantAdmin == null || string.IsNullOrEmpty(tenantAdmin.EmailAddress))
                        {
                            return;
                        }

                        var hostAdminLanguage = await _settingManager.GetSettingValueForUserAsync(
                            LocalizationSettingNames.DefaultLanguage, tenantAdmin.TenantId, tenantAdmin.Id);
                        var culture = CultureHelper.GetCultureInfoByChecking(hostAdminLanguage);
                        var expringEdition = await _editionManager.GetByIdAsync(expiringEditionId);
                        var emailTemplate = GetTitleAndSubTitle(tenantId, L("SubscriptionExpire_Title"),
                            L("SubscriptionExpire_SubTitle"));
                        var mailMessage = new StringBuilder();

                        mailMessage.AppendLine("<b>" + L("Message") + "</b>: " +
                                               L("SubscriptionAssignedToAnother_Email_Body", culture,
                                                   expringEdition.DisplayName, utcNow.ToString("yyyy-MM-dd") + " UTC") +
                                               "<br />");
                        mailMessage.AppendLine("<br />");

                        await ReplaceBodyAndSend(tenantAdmin.EmailAddress, L("SubscriptionExpire_Email_Subject"),
                            emailTemplate, mailMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        public async Task TryToSendFailedSubscriptionTerminationsEmail(List<string> failedTenancyNames, DateTime utcNow)
        {
            try
            {
                var hostAdmin = await _userManager.GetAdminAsync();
                if (hostAdmin == null || string.IsNullOrEmpty(hostAdmin.EmailAddress))
                {
                    return;
                }

                var hostAdminLanguage =
                    await _settingManager.GetSettingValueForUserAsync(LocalizationSettingNames.DefaultLanguage,
                        hostAdmin.TenantId, hostAdmin.Id);
                var culture = CultureHelper.GetCultureInfoByChecking(hostAdminLanguage);
                var emailTemplate = GetTitleAndSubTitle(null, L("FailedSubscriptionTerminations_Title"),
                    L("FailedSubscriptionTerminations_SubTitle"));
                var mailMessage = new StringBuilder();

                mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + L("FailedSubscriptionTerminations_Email_Body",
                    culture, string.Join(",", failedTenancyNames), utcNow.ToString("yyyy-MM-dd") + " UTC") + "<br />");
                mailMessage.AppendLine("<br />");

                await ReplaceBodyAndSend(hostAdmin.EmailAddress, L("FailedSubscriptionTerminations_Email_Subject"),
                    emailTemplate, mailMessage);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        public async Task TryToSendSubscriptionExpiringSoonEmail(int tenantId, DateTime dateToCheckRemainingDayCount)
        {
            try
            {
                using (_unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        var tenantAdmin = await _userManager.GetAdminAsync();
                        if (tenantAdmin == null || string.IsNullOrEmpty(tenantAdmin.EmailAddress))
                        {
                            return;
                        }

                        var tenantAdminLanguage =
                            await _settingManager.GetSettingValueForUserAsync(LocalizationSettingNames.DefaultLanguage,
                                tenantAdmin.TenantId, tenantAdmin.Id);
                        var culture = CultureHelper.GetCultureInfoByChecking(tenantAdminLanguage);

                        var emailTemplate = GetTitleAndSubTitle(null, L("SubscriptionExpiringSoon_Title"),
                            L("SubscriptionExpiringSoon_SubTitle"));
                        var mailMessage = new StringBuilder();

                        mailMessage.AppendLine("<b>" + L("Message") + "</b>: " +
                                               L("SubscriptionExpiringSoon_Email_Body", culture,
                                                   dateToCheckRemainingDayCount.ToString("yyyy-MM-dd") + " UTC") +
                                               "<br />");
                        mailMessage.AppendLine("<br />");

                        await ReplaceBodyAndSend(tenantAdmin.EmailAddress, L("SubscriptionExpiringSoon_Email_Subject"),
                            emailTemplate, mailMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        private string GetTenancyNameOrNull(int? tenantId)
        {
            if (tenantId == null)
            {
                return null;
            }

            using (_unitOfWorkProvider.Current.SetTenantId(null))
            {
                return _tenantRepository.Get(tenantId.Value).TenancyName;
            }
        }

        private StringBuilder GetTitleAndSubTitle(int? tenantId, string title, string subTitle)
        {
            var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(tenantId));
            emailTemplate.Replace("{EMAIL_TITLE}", title);
            emailTemplate.Replace("{EMAIL_SUB_TITLE}", subTitle);

            return emailTemplate;
        }

        private async Task ReplaceBodyAndSend(string emailAddress, string subject, StringBuilder emailTemplate,
            StringBuilder mailMessage)
        {
            emailTemplate.Replace("{EMAIL_BODY}", mailMessage.ToString());

            await _emailSender.SendAsync(new MailMessage
            {
                To = {emailAddress},
                Subject = subject,
                Body = emailTemplate.ToString(),
                IsBodyHtml = true
            });
 
        }

        /// <summary>
        /// Returns link with encrypted parameters
        /// </summary>
        /// <param name="link"></param>
        /// <param name="encrptedParameterName"></param>
        /// <returns></returns>
        private string EncryptQueryParameters(string link, string encrptedParameterName = "c")
        {
            if (!link.Contains("?"))
            {
                return link;
            }

            var basePath = link.Substring(0, link.IndexOf('?'));
            var query = link.Substring(link.IndexOf('?')).TrimStart('?');

            return basePath + "?" + encrptedParameterName + "=" +
                   HttpUtility.UrlEncode(SimpleStringCipher.Instance.Encrypt(query));
        }

        [Audited]
        public void SendFailedRecordPolicy(int tenantId, string failedTenancyNames, DateTime utcNow, List<string> softdeleteprojects, List<string> harddeleteprojects, List<string> archiveprojects, string exception)
        {
            
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var tenantAdmin = _userRepository.FirstOrDefault(i => i.NormalizedUserName.ToUpper() == AbpUserBase.AdminUserName.ToUpper() && i.TenantId == tenantId);

                if (tenantAdmin == null || string.IsNullOrEmpty(tenantAdmin.EmailAddress))
                {
                    return;
                }
                    
                var userName = tenantAdmin.Name;
                var tenantName = GetTenancyNameOrNull(tenantId);

                var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(tenantId));
                emailTemplate.Replace("{EMAIL_SUB_TITLE}", "");

                var body = $"<p>Hi {userName}</p>";
                body += $"<p>Your data retention policy failed to run at " + utcNow.ToString() + " UTC. </p>";
                body += $"<p>You can find detailed error messages in system logs.</p>";

                body += $"<p>The completed tasks are:</p>";

                body += $"<p><b>Archive</b></p>";
                body += $"<p>The following Active projects have been Archived as they meet the Archive retention conditions set in your tenancy.</p>";
                body += $"<ul>";
                if (archiveprojects.Count > 0)
                {
                    body += "<li>" + string.Join("<li> ", archiveprojects) + "<br/>";
                }
                body += $"</ul>";

                body += $"<p><b>Soft Delete</b></p>";
                body += $"<p>The following Archived projects have been Soft Deleted as they meet the Soft Delete retention conditions set in your tenancy. </p>";
                body += $"<ul>";
                if (softdeleteprojects.Count > 0)
                {
                    body += "<li>" + string.Join("<li> ", softdeleteprojects) + "<br/>";
                }
                body += $"</ul>";

                body += $"<p><b>Hard Delete</b></p>";
                body += $"<p>The following Soft Deleted projects have been Hard Deleted as they meet the Hard Delete retention conditions set in your tenancy. </p>";
                body += $"<ul>";
                if (harddeleteprojects.Count > 0)
                {
                    body += "<li>" + string.Join("<li> ", harddeleteprojects) + "<br/>";
                }
                body += $"</ul>";

                body += $"<p>You can modify your retention policies at any time by accessing you tenant admin account and clicking on Record Policy Actions.</p>";

                body += $"<p>This is an automated message from " + tenantName + ", please do not reply</p>";

                emailTemplate.Replace("{TENANT_NAME}", tenantName);
                emailTemplate.Replace("{EMAIL_BODY}", body);


                _AbpSession.Use(tenantId, null);
                _emailSender.Send(new MailMessage
                {
                    To = { tenantAdmin.EmailAddress },
                    Subject = L("FailedRunRecordPolicy_Title"),
                    Body = emailTemplate.ToString(),
                    IsBodyHtml = true
                });


                // TODO : We can remove that code  [It is only required for local - EMail is not sending locally through _emailSender.Send]
                EmailSendLocally(emailTemplate.ToString(), L("RunRecordPolicy_Title"), tenantAdmin.EmailAddress);
            }      
        }


        public void SendRunRecordPolicySuccessEmail(int tenantId, List<string> softdeleteprojects, List<string> harddeleteprojects, List<string> archiveprojects)
        {

            try
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {

 

                    var tenantAdmin = _userRepository.FirstOrDefault(i => i.NormalizedUserName.ToUpper() == AbpUserBase.AdminUserName.ToUpper() && i.TenantId == tenantId);
                    if (tenantAdmin == null || string.IsNullOrEmpty(tenantAdmin.EmailAddress))
                    {
                        return;
                    }
                    var userName = tenantAdmin.Name;
                    var tenantName = GetTenancyNameOrNull(tenantId);


                    var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(tenantId));
                    emailTemplate.Replace("{EMAIL_SUB_TITLE}", "");

                    var body = $"<p>Hi {userName}</p>";
                    body += $"<p>The following archive, soft delete and hard delete polices have been run against your account Projects.</p>";

                    body += $"<p><b>Archive</b></p>";
                    body += $"<p>The following Active projects have been Archived as they meet the Archive retention conditions set in your tenancy.</p>";
                    body += $"<ul>";
                    if (archiveprojects.Count > 0)
                    {
                        body += "<li> " + string.Join("<li> ", archiveprojects) + "<br/>";
                    }
                    body += $"</ul>";

                    body += $"<p><b>Soft Delete</b></p>";
                    body += $"<p>The following Archived projects have been Soft Deleted as they meet the Soft Delete retention conditions set in your tenancy. </p>";
                    body += $"<ul>";
                    if (softdeleteprojects.Count > 0)
                    {
                        body += "<li>" + string.Join("<li> ", softdeleteprojects) + "<br/>";
                    }
                    body += $"</ul>";

                    body += $"<p><b>Hard Delete</b></p>";
                    body += $"<p>The following Soft Deleted projects have been Hard Deleted as they meet the Hard Delete retention conditions set in your tenancy. </p>";
                    body += $"<ul>";
                    if (softdeleteprojects.Count > 0)
                    {
                        body += "<li>" + string.Join("<li> ", harddeleteprojects) + "<br/>";
                    }
                    body += $"</ul>";

                    body += $"<p>You can modify your retention policies at any time by accessing you tenant admin account and clicking on Record Policy Actions.</p>";

                    body += $"<p>This is an automated message from " + tenantName + ", please do not reply</p>";

                    emailTemplate.Replace("{TENANT_NAME}", tenantName);
                    emailTemplate.Replace("{EMAIL_BODY}", body);

                    _AbpSession.Use(tenantId, null);
                    _emailSender.Send(new MailMessage
                    {
                        To = { tenantAdmin.EmailAddress },
                        Subject = L("RunRecordPolicy_Title"),
                        Body = emailTemplate.ToString(),
                        IsBodyHtml = true
                    });
                }  
            }
            catch { }

         
        }

        // EMail Sending Locally (If is not working)
        private void EmailSendLocally(string Body, string Subject, string EMailTo)
        {
            using (MailMessage emailMessage = new MailMessage())
            {
                emailMessage.From = new MailAddress("admin@syntaq.com", "Syntaq Mailer (Test) 123");
                emailMessage.To.Add(new MailAddress(EMailTo, "Email"));
                emailMessage.Subject = Subject;
                emailMessage.Body = Body;
                emailMessage.Priority = MailPriority.Normal;
                emailMessage.IsBodyHtml = true;
                using (SmtpClient MailClient = new SmtpClient("in-v3.mailjet.com", 25))
                {
                    MailClient.EnableSsl = true;
                    MailClient.Credentials = new System.Net.NetworkCredential("9344e97647a12d23a03a0dbacf11e0ae", "b0c0e10e71f5c7682dd5f343220b7394");
                    MailClient.Send(emailMessage);
                }
            }
        }

        public string SendMannualReviewEmail(int? tenantId, string name)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var tenantName = GetTenancyNameOrNull(tenantId);

                var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(tenantId));
                emailTemplate.Replace("{EMAIL_SUB_TITLE}", "");

                //var body = $"<p>Dear {formdata.Client_Name.Name_First_txt}</p>";
                var body = $"<p>Dear {name}</p>";
                body += $"<p>We just wanted to let you know that your application to register a company has been referred within ASIC for manual review.  As a consequence, it’s likely to take a little bit longer to register your company - possibly up to 48 hours (and longer if there’s a weekend or public holiday).</p>";

                body += $"<p>We’ll be in touch as soon as we know the outcome.</p>";
                body += $"<p>Yours sincerely,</p>";

                body += $"<p></p>";
                body += $"<p>{tenantName}</p>";

                emailTemplate.Replace("{TENANT_NAME}", tenantName);
                emailTemplate.Replace("{EMAIL_BODY}", body);

                return emailTemplate.ToString();
                
            }
        }

        public string SendRejectEmail(int? tenantId, string name)
        {
            using (_unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var tenantName = GetTenancyNameOrNull(tenantId);

                    var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(tenantId));
                    emailTemplate.Replace("{EMAIL_SUB_TITLE}", "");

                    var body = $"<p>Dear {name}</p>";
                    body += $"<p>We just wanted to let you know that your application to register a company has been rejected by ASIC.  There can be a number of reasons for this, and most are relatively easy to rectify.</p>";

                    body += $"<p>Once our team have had an opportunity to understand why the application has been rejected, we’ll be in touch to discuss the next steps.  Depending on the complexity of the issue, we may contact you by email or by telephone.</p>";

                    body += $"<p>Please be patient as it can take possibly up to 48 hours to identify the issue and the next steps involved (and longer if there’s a weekend or public holiday).</p>";

                    body += $"<p>Yours sincerely,</p>";

                    body += $"<p></p>";
                    body += $"<p>{tenantName}</p>";

                    emailTemplate.Replace("{TENANT_NAME}", tenantName);
                    emailTemplate.Replace("{EMAIL_BODY}", body);

                    return emailTemplate.ToString();
                }
            }
        }


    }
}