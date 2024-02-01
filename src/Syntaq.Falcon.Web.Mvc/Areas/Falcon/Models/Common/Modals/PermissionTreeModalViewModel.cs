using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syntaq.Falcon.Authorization.Permissions.Dto;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Common.Modals
{
    public class PermissionTreeModalViewModel : IPermissionsEditViewModel
    {
        public List<FlatPermissionDto> Permissions { get; set; }
        public List<string> GrantedPermissionNames { get; set; }
    }
}
