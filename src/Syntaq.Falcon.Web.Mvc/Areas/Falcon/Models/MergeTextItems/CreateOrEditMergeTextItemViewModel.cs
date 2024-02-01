using Syntaq.Falcon.MergeTexts.Dtos;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.MergeTextItems
{
    public class CreateOrEditMergeTextItemModalViewModel
    {
       public CreateOrEditMergeTextItemDto MergeTextItem { get; set; }

	   		public string MergeTextItemValueKey { get; set;}


	   public bool IsEditMode => MergeTextItem.Id.HasValue;
    }
}