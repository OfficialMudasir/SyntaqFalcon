using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.RecordPolicies.Dtos
{
    public class GetAllRecordPoliciesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string NameFilter { get; set; }

    }
}