using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class CreateOrEditProjectTenantDto : EntityDto<Guid?>
    {

        public int SubscriberTenantId { get; set; }
        public string SubscriberTenantName { get; set; }
        public Guid ProjectId { get; set; }

        public bool Enabled { get; set; }

        public Guid? ProjectEnvironmentId { get; set; }

    }
}