using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Submissions.Dtos
{
	public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
	{
		public string Filter { get; set; }
	}
}