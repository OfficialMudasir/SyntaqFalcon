using Syntaq.Falcon.Forms.Dtos;

using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.FormFeedbacks
{
    public class CreateOrEditFormFeedbackModalViewModel
    {
       public CreateOrEditFormFeedbackDto FormFeedback { get; set; }

	   		public string FormName { get; set;}


       
	   public bool IsEditMode => FormFeedback.Id.HasValue;
    }
}