using System.Collections.Generic;
using Syntaq.Falcon.Authorization.Permissions.Dto;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Common
{
    public interface IPermissionsEditViewModel
    {
        List<FlatPermissionDto> Permissions { get; set; }

        List<string> GrantedPermissionNames { get; set; }
    }
}