using Syntaq.Falcon.MergeTexts.Dtos;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.MergeTextItemValues
{
    public class CreateOrEditMergeTextItemValueModalViewModel
    {
       public CreateOrEditMergeTextItemValueDto MergeTextItemValue { get; set; }

	   
	   public bool IsEditMode => MergeTextItemValue.Id.HasValue;
    }
}