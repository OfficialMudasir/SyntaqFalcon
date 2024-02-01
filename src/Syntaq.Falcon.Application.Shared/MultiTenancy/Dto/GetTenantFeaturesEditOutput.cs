using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Editions.Dto;

namespace Syntaq.Falcon.MultiTenancy.Dto
{
    public class GetTenantFeaturesEditOutput
    {
        public List<NameValueDto> FeatureValues { get; set; }

        public List<FlatFeatureDto> Features { get; set; }
    }
}