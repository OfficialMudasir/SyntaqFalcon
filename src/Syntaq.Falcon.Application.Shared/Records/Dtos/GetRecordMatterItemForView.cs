namespace Syntaq.Falcon.Records.Dtos
{
	public class GetRecordMatterItemForView
	{
		public RecordMatterItemDto RecordMatterItem { get; set; }
		public string RecordMatterTenantId { get; set;}
	}

	public class GetRecordMatterItemForDownload
	{
		public RecordMatterItemForDownloadDto RecordMatterItem { get; set; }
		public string RecordMatterTenantId { get; set; }
	}
}