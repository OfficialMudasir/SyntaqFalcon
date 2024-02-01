using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.EntityVersionHistories.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}