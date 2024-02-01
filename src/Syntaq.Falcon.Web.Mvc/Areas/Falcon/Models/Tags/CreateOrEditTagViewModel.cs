using Syntaq.Falcon.Tags.Dtos;

using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Tags
{
    public class CreateOrEditTagModalViewModel
    {
        public CreateOrEditTagDto Tag { get; set; }

        public bool IsEditMode => Tag.Id.HasValue;
    }
}