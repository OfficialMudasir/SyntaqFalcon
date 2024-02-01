using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Syntaq.Falcon.Teams.Dtos
{
    public class CreateOrUpdateTeamUserRoleInput 
    {
        //public long OrganizationUnitId { get; set; }
        //[Required]
        public TeamUserRoleDto TeamUser { get; set; }

        //[Required]
        //public List<KeyValuePair<string, string>> AssignedRoleNames { get; set; }

        public List<AssignedTeamUserRolesDto> AssignedTeamUserRoles { get; set; }
    }
}
