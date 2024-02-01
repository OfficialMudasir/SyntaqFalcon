using Abp.AutoMapper;
using Syntaq.Falcon.Sessions.Dto;

namespace Syntaq.Falcon.Web.Views.Shared.Components.TenantChange
{
    [AutoMapFrom(typeof(GetCurrentLoginInformationsOutput))]
    public class TenantChangeViewModel
    {
        public TenantLoginInfoDto Tenant { get; set; }
    }
}