using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using Syntaq.Falcon.Authorization.Users.Dto;

namespace Syntaq.Falcon.Teams.Dtos
{
    public class GetTeamForEditOutput
    {
		public CreateOrEditTeamDto Team { get; set; }

        public UserRoleDto[] Roles { get; set; }
    }
}