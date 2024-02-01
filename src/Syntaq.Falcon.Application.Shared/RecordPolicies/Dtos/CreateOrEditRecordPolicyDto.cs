using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.RecordPolicies.Dtos
{
    public class CreateOrEditRecordPolicyDto : EntityDto<Guid?>
    {

        [Required]
        [StringLength(RecordPolicyConsts.MaxNameLength, MinimumLength = RecordPolicyConsts.MinNameLength)]
        public string Name { get; set; }

        public int AppliedTenantId { get; set; }

    }
}