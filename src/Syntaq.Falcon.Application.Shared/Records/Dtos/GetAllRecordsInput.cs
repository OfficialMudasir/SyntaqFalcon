using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Records.Dtos
{
	public class GetAllRecordsInput : PagedAndSortedResultRequestDto
	{
		public string Filter { get; set; }

		public string RecordNameFilter { get; set; }

		public string Id { get; set; } 
		//public string FolderId { get; set; }

		public string Type { get; set; }

		public bool IsArchived { get; set; }
	}
}