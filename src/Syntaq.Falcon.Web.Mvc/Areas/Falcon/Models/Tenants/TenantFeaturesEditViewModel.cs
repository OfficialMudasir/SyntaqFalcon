using Abp.AutoMapper;
using Syntaq.Falcon.MultiTenancy;
using Syntaq.Falcon.MultiTenancy.Dto;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Common;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Tenants
{
    [AutoMapFrom(typeof (GetTenantFeaturesEditOutput))]
    public class TenantFeaturesEditViewModel : GetTenantFeaturesEditOutput, IFeatureEditViewModel
    {
        public Tenant Tenant { get; set; }
    }
}