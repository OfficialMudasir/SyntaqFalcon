using Syntaq.Falcon.Tags;

using System;
using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace Syntaq.Falcon.Tags.Dtos
{
    public class TagEntityTypeDto : EntityDto<Guid>
    {
        public EntityType EntityType { get; set; }

        public Guid? TagId { get; set; }

        public IEnumerable<TagValueDto> Values { get; set; }
    }
}