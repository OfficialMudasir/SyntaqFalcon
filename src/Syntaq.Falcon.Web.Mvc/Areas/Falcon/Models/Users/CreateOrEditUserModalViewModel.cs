using System.Linq;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Syntaq.Falcon.Authorization.Users.Dto;
using Syntaq.Falcon.Security;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Common;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Users
{
    [AutoMapFrom(typeof(GetUserForEditOutput))]
    public class CreateOrEditUserModalViewModel : GetUserForEditOutput, IOrganizationUnitsEditViewModel
    {
        public bool CanChangeUserName => User.UserName != AbpUserBase.AdminUserName;

        public int AssignedRoleCount
        {
            get { return Roles.Count(r => r.IsAssigned); }
        }

        public bool IsEditMode => User.Id.HasValue;

        public PasswordComplexitySetting PasswordComplexitySetting { get; set; }

        public string AllowedUserNameCharacters { get; set; }
    }
}