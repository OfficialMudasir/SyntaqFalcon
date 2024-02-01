using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Tags.Dtos
{
    public class CreateOrEditTagDto : EntityDto<Guid?>
    {

        [StringLength(TagConsts.MaxNameLength, MinimumLength = TagConsts.MinNameLength)]
        public string Name { get; set; }

    }
}