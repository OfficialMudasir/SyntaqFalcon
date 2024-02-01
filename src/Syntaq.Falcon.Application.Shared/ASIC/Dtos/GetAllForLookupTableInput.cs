using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.ASIC.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}