using System;
using Abp.Notifications;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Notifications.Dto
{
    public class GetUserNotificationsInput : PagedInputDto
    {
        public UserNotificationState? State { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}