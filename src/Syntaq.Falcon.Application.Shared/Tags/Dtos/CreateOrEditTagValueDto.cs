using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Tags.Dtos
{
    public class CreateOrEditTagValueDto : EntityDto<Guid?>
    {

        [StringLength(TagValueConsts.MaxValueLength, MinimumLength = TagValueConsts.MinValueLength)]
        public string Value { get; set; }

        public Guid? TagId { get; set; }

    }
}