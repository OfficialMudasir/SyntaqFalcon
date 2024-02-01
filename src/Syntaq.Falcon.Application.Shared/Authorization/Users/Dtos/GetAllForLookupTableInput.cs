using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Authorization.Users.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}