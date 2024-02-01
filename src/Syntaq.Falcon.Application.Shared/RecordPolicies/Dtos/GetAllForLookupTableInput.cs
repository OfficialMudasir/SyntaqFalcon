using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.RecordPolicies.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}