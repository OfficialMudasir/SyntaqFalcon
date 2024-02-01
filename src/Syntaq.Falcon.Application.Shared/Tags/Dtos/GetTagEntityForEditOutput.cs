using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Tags.Dtos
{
    public class GetTagEntityForEditOutput
    {
        public CreateOrEditTagEntityDto TagEntity { get; set; }

        public string TagValueValue { get; set; }

    }
}