using Abp.AutoMapper;
using Syntaq.Falcon.MultiTenancy.Dto;

namespace Syntaq.Falcon.Web.Models.TenantRegistration
{
    [AutoMapFrom(typeof(RegisterTenantOutput))]
    public class TenantRegisterResultViewModel : RegisterTenantOutput
    {
        public string TenantLoginAddress { get; set; }
    }
}