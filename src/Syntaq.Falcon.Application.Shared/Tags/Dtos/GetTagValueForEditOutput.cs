using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Tags.Dtos
{
    public class GetTagValueForEditOutput
    {
        public CreateOrEditTagValueDto TagValue { get; set; }

        public string TagName { get; set; }

    }
}