using Syntaq.Falcon.Tags;

using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Tags.Dtos
{
    public class TagEntityDto : EntityDto<Guid>
    {
        public Guid EntityId { get; set; }

        public EntityType EntityType { get; set; }

        public Guid? TagValueId { get; set; }

    }
}