using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Users.Dtos
{
    public class GetAllUserAcceptancesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string AcceptedFilter { get; set; }

		public string UserAcceptanceTypeNameFilter { get; set; }

		public string TenantNameFilter { get; set; }
		public string UserNameFilter { get; set; }
		public string UserFirstNameFilter { get; set; }
		public string UserSurnameFilter { get; set; }
		public string UserEmailFilter { get; set; }

		public string RecordMatterContributorNameFilter { get; set; }

		 
    }
}