using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.RecordPolicyActions.Dtos
{
    public class GetAllRecordPolicyActionsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string NameFilter { get; set; }

        public int? MaxExpireDaysFilter { get; set; }
        public int? MinExpireDaysFilter { get; set; }

        public int? ActiveFilter { get; set; }

        public string RecordPolicyNameFilter { get; set; }
        public Guid RecordPolicyId { get; set; }
    }
}