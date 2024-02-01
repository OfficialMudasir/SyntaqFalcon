using Syntaq.Falcon.Apps.Dtos;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Apps
{
    public class CreateOrEditAppModalViewModel
    {
       public CreateOrEditAppDto App { get; set; }

	   
	   public bool IsEditMode => App.Id.HasValue;
    }
}