using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Records.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}