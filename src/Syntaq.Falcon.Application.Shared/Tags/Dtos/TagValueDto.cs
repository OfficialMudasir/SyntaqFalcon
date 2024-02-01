using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Tags.Dtos
{
    public class TagValueDto : EntityDto<Guid>
    {
        public string Value { get; set; }

        public Guid? TagId { get; set; }

    }
}