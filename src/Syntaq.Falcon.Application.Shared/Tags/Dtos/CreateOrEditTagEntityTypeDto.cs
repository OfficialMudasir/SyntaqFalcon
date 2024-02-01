using Syntaq.Falcon.Tags;

using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Tags.Dtos
{
    public class CreateOrEditTagEntityTypeDto : EntityDto<Guid?>
    {

        public EntityType EntityType { get; set; }

        public Guid? TagId { get; set; }

    }
}