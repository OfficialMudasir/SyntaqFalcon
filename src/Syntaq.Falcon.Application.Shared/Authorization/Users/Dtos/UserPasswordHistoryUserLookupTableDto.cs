using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Authorization.Users.Dtos
{
    public class UserPasswordHistoryUserLookupTableDto
    {
        public long Id { get; set; }

        public string DisplayName { get; set; }
    }
}