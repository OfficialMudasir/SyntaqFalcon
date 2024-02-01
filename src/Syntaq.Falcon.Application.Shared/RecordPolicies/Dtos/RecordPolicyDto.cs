using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.RecordPolicies.Dtos
{
    public class RecordPolicyDto : EntityDto<Guid>
    {
        public string Name { get; set; }

        public int AppliedTenantId { get; set; }

    }
}