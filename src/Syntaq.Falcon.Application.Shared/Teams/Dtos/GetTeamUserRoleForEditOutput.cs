using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using Syntaq.Falcon.Authorization.Users.Dto;

namespace Syntaq.Falcon.Teams.Dtos
{
    public class GetTeamUserRoleForEditOutput
    {
		//public CreateOrEditTeamUserRoleDto TeamUserRole { get; set; }

        public TeamUserRoleDto TeamUserRole { get; set; }

        public UserRoleDto[] Roles { get; set; }
    }
}