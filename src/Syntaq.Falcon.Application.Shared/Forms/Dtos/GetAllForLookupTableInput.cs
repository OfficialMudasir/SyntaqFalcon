using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Forms.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}