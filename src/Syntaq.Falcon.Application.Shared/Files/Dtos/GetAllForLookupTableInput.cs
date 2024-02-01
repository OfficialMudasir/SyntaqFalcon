using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Files.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}