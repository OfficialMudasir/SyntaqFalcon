using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Teams.Dtos
{
    public class TeamUserRoleDto : AuditedEntityDto<int>
    {
        public int? TenantId { get; set; }

        public int RoleId { get; set; }

        public long OrganizationUnitId { get; set; }

        public long UserId { get; set; }

		 
    }
}