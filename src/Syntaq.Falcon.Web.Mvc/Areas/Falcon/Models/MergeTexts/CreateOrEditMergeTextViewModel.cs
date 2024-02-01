using Syntaq.Falcon.MergeTexts.Dtos;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.MergeTexts
{
    public class CreateOrEditMergeTextModalViewModel
    {
       public CreateOrEditMergeTextDto MergeText { get; set; }

	   		public string MergeTextItemName { get; set;}


	   public bool IsEditMode => MergeText.Id.HasValue;
    }
}