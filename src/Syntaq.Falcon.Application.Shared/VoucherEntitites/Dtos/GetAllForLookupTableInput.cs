using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.VoucherEntitites.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}