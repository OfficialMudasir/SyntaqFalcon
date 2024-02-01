using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Users.Dtos
{
    public class UserAcceptanceUserLookupTableDto
    {
		public long? Id { get; set; }  //userId

		public string DisplayName { get; set; }

        public string SurName { get; set; }

        public string Email { get; set; }

        public string TenancyCodeName { get; set; }

        public int? TenantId { get; set; }
    }
}