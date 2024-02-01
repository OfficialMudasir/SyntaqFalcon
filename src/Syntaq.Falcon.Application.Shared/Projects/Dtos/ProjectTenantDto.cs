using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class ProjectTenantDto : EntityDto<Guid>
    {
        public int SubscriberTenantId { get; set; }
        public string SubscriberTenantName { get; set; }

        public Guid ProjectId { get; set; }

        public bool Enabled { get; set; }

        public Guid? ProjectEnvironmentId { get; set; }

    }
}