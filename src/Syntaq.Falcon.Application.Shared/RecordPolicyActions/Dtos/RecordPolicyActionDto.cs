using System;
using Abp.Application.Services.Dto;
using static Syntaq.Falcon.RecordPolicyActions.RecordPolicyActionConsts;

namespace Syntaq.Falcon.RecordPolicyActions.Dtos
{
    public class RecordPolicyActionDto : EntityDto<Guid>
    {
        public string Name { get; set; }

        public int AppliedTenantId { get; set; }

        public int ExpireDays { get; set; }

        public bool Active { get; set; }

        public RecordPolicyActionType Type { get; set; }

        public RecordStatusType RecordStatus { get; set; }

        public Guid RecordPolicyId { get; set; }

    }
}