using System.Collections.Generic;
using Syntaq.Falcon.Authorization.Permissions.Dto;

namespace Syntaq.Falcon.Authorization.Users.Dto
{
    public class GetUserPermissionsForEditOutput
    {
        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}