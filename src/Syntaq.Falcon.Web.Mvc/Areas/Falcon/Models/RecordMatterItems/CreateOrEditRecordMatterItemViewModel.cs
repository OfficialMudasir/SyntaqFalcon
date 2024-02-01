using Syntaq.Falcon.Records.Dtos;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.RecordMatterItems
{
	public class CreateOrEditRecordMatterItemModalViewModel
	{
		public CreateOrEditRecordMatterItemDto RecordMatterItem { get; set; }
		public string RecordMatterTenantId { get; set;}
		public bool IsEditMode => RecordMatterItem.Id.HasValue;
	}
}