using Syntaq.Falcon.Tags.Dtos;

using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.TagValues
{
    public class CreateOrEditTagValueModalViewModel
    {
        public CreateOrEditTagValueDto TagValue { get; set; }

        public string TagName { get; set; }

        public bool IsEditMode => TagValue.Id.HasValue;
    }
}