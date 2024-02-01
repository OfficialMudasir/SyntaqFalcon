using Syntaq.Falcon.Users.Dtos;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.UserAcceptanceTypes
{
    public class CreateOrEditUserAcceptanceTypeModalViewModel
    {
       public CreateOrEditUserAcceptanceTypeDto UserAcceptanceType { get; set; }

	   		public string TemplateName { get; set;}


	   public bool IsEditMode => UserAcceptanceType.Id.HasValue;
    }
}