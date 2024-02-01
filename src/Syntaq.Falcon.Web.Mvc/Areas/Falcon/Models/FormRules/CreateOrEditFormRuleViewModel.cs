using Syntaq.Falcon.Forms.Dtos;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.FormRules
{
    public class CreateOrEditFormRuleModalViewModel
    {
        public CreateOrEditFormRuleDto FormRule { get; set; }

	    public string FormName { get; set;}

        //public bool IsEditMode => FormRule.Id.HasValue;
        public bool IsEditMode = true;
    }
}