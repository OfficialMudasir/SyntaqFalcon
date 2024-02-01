using Abp.AutoMapper;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Authorization.Users.Dto;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Common;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Users
{
    [AutoMapFrom(typeof(GetUserPermissionsForEditOutput))]
    public class UserPermissionsEditViewModel : GetUserPermissionsForEditOutput, IPermissionsEditViewModel
    {
        public User User { get; set; }
    }
}