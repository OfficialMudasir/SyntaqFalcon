using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Documents.Dtos
{
	public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
	{
		public string Filter { get; set; }
	}
}