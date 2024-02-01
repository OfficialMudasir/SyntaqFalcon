using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.MergeTexts.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}