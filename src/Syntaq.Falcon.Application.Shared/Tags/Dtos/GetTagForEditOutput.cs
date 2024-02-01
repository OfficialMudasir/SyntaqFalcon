using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Tags.Dtos
{
    public class GetTagForEditOutput
    {
        public CreateOrEditTagDto Tag { get; set; }

    }
}