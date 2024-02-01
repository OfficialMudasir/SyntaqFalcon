using Syntaq.Falcon.Records.Dtos;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Records
{
	public class CreateOrEditRecordModalViewModel
	{
	   public CreateOrEditRecordDto Record { get; set; }

	   
	   public bool IsEditMode => Record.Id.HasValue;
	}
}