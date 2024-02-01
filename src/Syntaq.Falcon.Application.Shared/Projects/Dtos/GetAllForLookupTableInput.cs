using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}