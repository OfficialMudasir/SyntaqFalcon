using Abp.Application.Services.Dto;
using Syntaq.Falcon.Tags;
using Syntaq.Falcon.Tags.Dtos;
using System;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.TagValues
{
    public class CreateOrEditTagEntitiesModalViewModel
    {

        public Guid EntityId { get; set; }

        public bool IsEditMode = true; // => TagValue.Id.HasValue;

        public PagedResultDto<GetTagEntityTypeForViewDto> Tags { get; set; }
        public PagedResultDto<GetTagEntityForViewDto> EntityTags { get; set; }


    }
}