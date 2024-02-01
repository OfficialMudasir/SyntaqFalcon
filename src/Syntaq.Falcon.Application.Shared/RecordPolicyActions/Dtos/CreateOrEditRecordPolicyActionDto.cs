using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using static Syntaq.Falcon.RecordPolicyActions.RecordPolicyActionConsts;

namespace Syntaq.Falcon.RecordPolicyActions.Dtos
{
    public class CreateOrEditRecordPolicyActionDto : EntityDto<Guid?>
    {

        [Required]
        [StringLength(RecordPolicyActionConsts.MaxNameLength, MinimumLength = RecordPolicyActionConsts.MinNameLength)]
        public string Name { get; set; }

        public int AppliedTenantId { get; set; }

        public int ExpireDays { get; set; }

        public bool Active { get; set; }

        public RecordPolicyActionType Type { get; set; }

        public RecordStatusType RecordStatus { get; set; }

        public Guid RecordPolicyId { get; set; }

    }
}