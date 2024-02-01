using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.RecordPolicyActions.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}