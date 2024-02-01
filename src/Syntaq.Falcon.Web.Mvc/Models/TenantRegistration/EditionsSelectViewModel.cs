using Abp.AutoMapper;
using Syntaq.Falcon.MultiTenancy.Dto;

namespace Syntaq.Falcon.Web.Models.TenantRegistration
{
    [AutoMapFrom(typeof(EditionsSelectOutput))]
    public class EditionsSelectViewModel : EditionsSelectOutput
    {
    }
}
