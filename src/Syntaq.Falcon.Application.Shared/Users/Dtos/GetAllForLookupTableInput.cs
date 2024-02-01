using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Users.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}