using System;
using System.Collections.Generic;
using Abp.Organizations;
using Syntaq.Falcon.Authorization.Users;
using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Syntaq.Falcon.Notifications.Dto
{
    public class NotificationRecipientsDTO
    {
        public int? Id { get; set; }
        public string value { get; set; }
        public string Type { get; set; }
    }
}
