using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Notifications;
using Abp.Runtime.Session;
using Abp.UI;
using Syntaq.Falcon.Notifications.Dto;
using Syntaq.Falcon.Net.Emailing;
using Abp.Net.Mail;
using System.Text;
using Syntaq.Falcon.Authorization.Users;
using System.Net.Mail;
using Newtonsoft.Json;
using Syntaq.Falcon.Organizations.Dto;
using Syntaq.Falcon.Organizations;
using NUglify.Helpers;
using Abp.MultiTenancy;

namespace Syntaq.Falcon.Notifications
{
    [AbpAuthorize]
    public class NotificationAppService : FalconAppServiceBase, INotificationAppService
    {
        private readonly INotificationDefinitionManager _notificationDefinitionManager;
        private readonly IUserNotificationManager _userNotificationManager;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplateProvider _emailTemplateProvider;
        private readonly IAppNotifier _appNotifier;
        private readonly IOrganizationUnitAppService _organizationUnitAppService;
        private readonly ITenantCache _tenantCache;

        public NotificationAppService(
            INotificationDefinitionManager notificationDefinitionManager,
            IUserNotificationManager userNotificationManager,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IEmailSender emailSender,
            IEmailTemplateProvider emailTemplateProvider,
            IAppNotifier appNotifier,
            ITenantCache tenantCache,
            IOrganizationUnitAppService organizationUnitAppService)
        {
            _notificationDefinitionManager = notificationDefinitionManager;
            _userNotificationManager = userNotificationManager;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _emailSender = emailSender;
            _emailTemplateProvider = emailTemplateProvider;
            _appNotifier = appNotifier;
            _organizationUnitAppService = organizationUnitAppService;
            _tenantCache = tenantCache;
        }

        [DisableAuditing]
        public async Task<GetNotificationsOutput> GetUserNotifications(GetUserNotificationsInput input)
        {
            var totalCount = await _userNotificationManager.GetUserNotificationCountAsync(
                AbpSession.ToUserIdentifier(), input.State, input.StartDate, input.EndDate
                );

            var unreadCount = await _userNotificationManager.GetUserNotificationCountAsync(
                AbpSession.ToUserIdentifier(), UserNotificationState.Unread, input.StartDate, input.EndDate
                );
            var notifications = await _userNotificationManager.GetUserNotificationsAsync(
                AbpSession.ToUserIdentifier(), input.State, input.SkipCount, input.MaxResultCount, input.StartDate, input.EndDate
                );

            return new GetNotificationsOutput(totalCount, unreadCount, notifications);
        }

        public async Task SetAllNotificationsAsRead()
        {
            await _userNotificationManager.UpdateAllUserNotificationStatesAsync(AbpSession.ToUserIdentifier(), UserNotificationState.Read);
        }

        public async Task<SetNotificationAsReadOutput> SetNotificationAsRead(EntityDto<Guid> input)
        {
            var userNotification = await _userNotificationManager.GetUserNotificationAsync(AbpSession.TenantId, input.Id);
            if (userNotification == null)
            {
                return new SetNotificationAsReadOutput(false);
            }

            if (userNotification.UserId != AbpSession.GetUserId())
            {
                throw new Exception(
                    $"Given user notification id ({input.Id}) is not belong to the current user ({AbpSession.GetUserId()})"
                );
            }

            if (userNotification.State == UserNotificationState.Read)
            {
                return new SetNotificationAsReadOutput(false);
            }
            
            await _userNotificationManager.UpdateUserNotificationStateAsync(AbpSession.TenantId, input.Id, UserNotificationState.Read);
            return new SetNotificationAsReadOutput(true);
        }

        public async Task<GetNotificationSettingsOutput> GetNotificationSettings()
        {
            var output = new GetNotificationSettingsOutput();

            output.ReceiveNotifications = await SettingManager.GetSettingValueAsync<bool>(NotificationSettingNames.ReceiveNotifications);

            //Get general notifications, not entity related notifications.
            var notificationDefinitions = (await _notificationDefinitionManager.GetAllAvailableAsync(AbpSession.ToUserIdentifier())).Where(nd => nd.EntityType == null);

            output.Notifications = ObjectMapper.Map<List<NotificationSubscriptionWithDisplayNameDto>>(notificationDefinitions);

            var subscribedNotifications = (await _notificationSubscriptionManager
                .GetSubscribedNotificationsAsync(AbpSession.ToUserIdentifier()))
                .Select(ns => ns.NotificationName)
                .ToList();

            output.Notifications.ForEach(n => n.IsSubscribed = subscribedNotifications.Contains(n.Name));

            return output;
        }

        public async Task UpdateNotificationSettings(UpdateNotificationSettingsInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), NotificationSettingNames.ReceiveNotifications, Convert.ToString(input.ReceiveNotifications));

            foreach (var notification in input.Notifications)
            {
                if (notification.IsSubscribed)
                {
                    await _notificationSubscriptionManager.SubscribeAsync(AbpSession.ToUserIdentifier(), notification.Name);
                }
                else
                {
                    await _notificationSubscriptionManager.UnsubscribeAsync(AbpSession.ToUserIdentifier(), notification.Name);
                }
            }
        }

        public async Task DeleteNotification(EntityDto<Guid> input)
        {
            var notification = await _userNotificationManager.GetUserNotificationAsync(AbpSession.TenantId, input.Id);
            if (notification == null)
            {
                return;
            }

            if (notification.UserId != AbpSession.GetUserId())
            {
                throw new UserFriendlyException(L("ThisNotificationDoesntBelongToYou"));
            }

            await _userNotificationManager.DeleteUserNotificationAsync(AbpSession.TenantId, input.Id);
        }

        public async Task DeleteAllUserNotifications(DeleteAllUserNotificationsInput input)
        {
            await _userNotificationManager.DeleteAllUserNotificationsAsync(
                AbpSession.ToUserIdentifier(),
                input.State,
                input.StartDate,
                input.EndDate);
        }

        public async Task FormTemplateUpdateNotification(string message, string emailBody, dynamic recipientsDTO, string notificationChannel, string entityName)
        {
            List<NotificationRecipientsDTO> Recipients = JsonConvert.DeserializeObject<List<NotificationRecipientsDTO>>(Convert.ToString(recipientsDTO.Assignees));
            var users = new List<User>();
            Recipients.ForEach(i =>
            {
                try
                {
                    if (i.Type == "User")
                    {
                        var user = UserManager.GetUserById((long)i.Id);
                        users.Add(user);
                    }
                    else  // Team 
                    {
                        var output = _organizationUnitAppService.GetOrganizationUnitUsersSync(new GetOrganizationUnitUsersInput { Id = (long)i.Id });
                        output.Items.ForEach(j =>
                        {
                            var user = UserManager.GetUserById(j.Id);
                            if (!users.Contains(user))
                            {
                                users.Add(user);
                            }
                        });
                    }
                }
                catch (Exception)
                {
                    throw new UserFriendlyException("Couldn't Notify: " + i.Type == "User" ? "User" : "Team" + " not found.");
                }
            });

            foreach (var user in users)
                {
                    List<NotificationSubscription> subscriptions = _notificationSubscriptionManager.GetSubscribedNotifications(user.ToUserIdentifier());
                    bool notificationSubscrided = subscriptions.Any(NotificationSubscription => NotificationSubscription.NotificationName.Equals(AppNotificationNames.FormTemplateUpdate, StringComparison.Ordinal));
                    if (notificationSubscrided)
                    {
                         await _appNotifier.CreateOrUpdateTemplateMessage(user, message);
                        if (notificationChannel == "EmailPlatform")
                        {
                            await NotificationViaEmail(user.EmailAddress, emailBody, message, entityName);
                        }
                    }
                }
        }

        public async Task NotificationViaEmail(string recipientEmail, string emailBody, string message, string entityName)
        {

            //var tenancyNameOrNull = AbpSession.TenantId.HasValue ? _tenantCache.GetOrNull((int)AbpSession.TenantId)?.TenancyName : null;
            var tenancyNameOrNull = AbpSession.TenantId.HasValue ? TenantManager.GetTenantName((int)AbpSession.TenantId).Result : "host";

            var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(AbpSession.TenantId));

            emailTemplate.Replace("{EMAIL_SUB_TITLE}", "");
            //emailTemplate.Replace("{TENANT_NAME}", "");
            if (emailBody.Contains("\n") || emailBody.Contains("\r") || emailBody.Contains("\r\n"))
                emailBody = emailBody.Replace("\r\n", "<br>").Replace("\n", "<br>").Replace("\r", "<br>");
            var body = $"{emailBody}";
            emailTemplate.Replace("{TENANT_NAME}", tenancyNameOrNull);
            emailTemplate.Replace("{EMAIL_BODY}", body);

            MailMessage mail = new MailMessage
            {
                Subject = $"Update notification: {message.Substring(0, message.IndexOf(':')).ToLower()} - {entityName} changed",
                Body = Convert.ToString(emailTemplate),
                IsBodyHtml = true 
            };
            mail.To.Add(new MailAddress(recipientEmail));
            var emailfrom = SettingManager.GetSettingValue("Abp.Net.Mail.DefaultFromAddress");
            mail.From = new MailAddress(emailfrom);
            await _emailSender.SendAsync(mail);
        }

    }
}
