using Syntaq.Falcon.Tags.Dtos;

using Abp.Extensions;
using System;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.TagEntityTypes
{
    public class CreateOrEditTagEntityTypeModalViewModel
    {
        public CreateOrEditTagEntityTypeDto TagEntityType { get; set; }

        public string TagName { get; set; }

        public bool IsEditMode => TagEntityType.Id.HasValue;

        public string projectBinded { get; set; }

        public Guid? tagId { get; set; }
    }
}