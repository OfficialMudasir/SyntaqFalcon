using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.VoucherUsages.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}