using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Tags.Dtos
{
    public class TagDto : EntityDto<Guid>
    {
        public string Name { get; set; }

    }
}