using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Tags.Dtos
{
    public class GetTagEntityTypeForEditOutput
    {
        public CreateOrEditTagEntityTypeDto TagEntityType { get; set; }

        public string TagName { get; set; }

    }
}