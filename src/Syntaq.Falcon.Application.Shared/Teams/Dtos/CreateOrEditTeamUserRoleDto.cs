
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using Syntaq.Falcon.Authorization.Users.Dto;

namespace Syntaq.Falcon.Teams.Dtos
{
    public class CreateOrEditTeamUserRoleDto : AuditedEntityDto<long?>
    {
		public long OrganizationUnitId { get; set; }
		 
		public long UserId { get; set; }

        //public string OrganizationUnitDisplayName { get; set; }

        //public string UserName { get; set; }
    }
}