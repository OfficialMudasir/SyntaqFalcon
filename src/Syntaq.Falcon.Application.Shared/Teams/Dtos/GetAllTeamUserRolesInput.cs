using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Teams.Dtos
{
    public class GetAllTeamUserRolesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string OrganizationUnitDisplayNameFilter { get; set; }

		 		 public string UserNameFilter { get; set; }

		 
    }
}