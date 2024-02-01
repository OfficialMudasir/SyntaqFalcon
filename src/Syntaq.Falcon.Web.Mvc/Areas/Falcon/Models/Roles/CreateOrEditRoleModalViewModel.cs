using Abp.AutoMapper;
using Syntaq.Falcon.Authorization.Roles.Dto;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Common;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Roles
{
    [AutoMapFrom(typeof(GetRoleForEditOutput))]
    public class CreateOrEditRoleModalViewModel : GetRoleForEditOutput, IPermissionsEditViewModel
    {
        public bool IsEditMode => Role.Id.HasValue;
    }
}