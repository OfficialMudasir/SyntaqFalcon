using Syntaq.Falcon.Tags.Dtos;

using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.TagEntities
{
    public class CreateOrEditTagEntityModalViewModel
    {
        public CreateOrEditTagEntityDto TagEntity { get; set; }

        public string TagValueValue { get; set; }

        public bool IsEditMode => TagEntity.Id.HasValue;
    }
}