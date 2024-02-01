using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Tags.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}