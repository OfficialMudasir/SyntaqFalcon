using Syntaq.Falcon.Tags;

using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Tags.Dtos
{
    public class CreateOrEditTagEntityDto : EntityDto<Guid?>
    {

        public Guid EntityId { get; set; }

        public EntityType EntityType { get; set; }

        public Guid? TagValueId { get; set; }

        public bool IsSelected { get; set; }

    }
}