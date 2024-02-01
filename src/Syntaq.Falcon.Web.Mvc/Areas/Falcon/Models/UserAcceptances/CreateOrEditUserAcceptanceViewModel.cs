using Syntaq.Falcon.Users.Dtos;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.UserAcceptances
{
    public class CreateOrEditUserAcceptanceModalViewModel
    {
        public CreateOrEditUserAcceptanceDto UserAcceptance { get; set; }

	    public string UserAcceptanceTypeName { get; set;}

		public string UserName { get; set;}

		public string RecordMatterContributorName { get; set;}


	   public bool IsEditMode => UserAcceptance.Id.HasValue;
    }
}