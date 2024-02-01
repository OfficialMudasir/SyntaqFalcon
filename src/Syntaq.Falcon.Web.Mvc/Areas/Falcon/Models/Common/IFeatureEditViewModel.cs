using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Editions.Dto;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Common
{
    public interface IFeatureEditViewModel
    {
        List<NameValueDto> FeatureValues { get; set; }

        List<FlatFeatureDto> Features { get; set; }
    }
}