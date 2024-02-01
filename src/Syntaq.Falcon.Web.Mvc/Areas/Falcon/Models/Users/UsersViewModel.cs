using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Authorization.Permissions.Dto;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Common;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Users
{
    public class UsersViewModel : IPermissionsEditViewModel
    {
        public string FilterText { get; set; }

        public List<ComboboxItemDto> Roles { get; set; }

        public bool OnlyLockedUsers { get; set; }

        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}
