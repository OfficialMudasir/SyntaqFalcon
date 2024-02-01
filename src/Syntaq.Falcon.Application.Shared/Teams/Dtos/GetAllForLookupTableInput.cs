using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Teams.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}