using Abp.AutoMapper;
using Syntaq.Falcon.AccessControlList.Dtos;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.AccessControlList
{
    [AutoMapFrom(typeof(GetACLForView))]
    public class ManageACLModalViewModel : GetACLForView
    {
        public ManageACLModalViewModel(GetACLForView output)
        {
            output.MapTo(this);
        }

        public bool SimpleView { get; set; } = false;
    }
}
