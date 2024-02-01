using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class GetAllProjectTenantsForExcelInput
    {
        public string Filter { get; set; }

        public int? MaxSubscriberTenantIdFilter { get; set; }
        public int? MinSubscriberTenantIdFilter { get; set; }

        public Guid? ProjectIdFilter { get; set; }

        public int? EnabledFilter { get; set; }

        public string ProjectEnvironmentNameFilter { get; set; }

    }
}