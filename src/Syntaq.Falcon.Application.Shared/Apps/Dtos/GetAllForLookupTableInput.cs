using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Apps.Dtos
{
	public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
	{
		public string Filter { get; set; }
	}
}