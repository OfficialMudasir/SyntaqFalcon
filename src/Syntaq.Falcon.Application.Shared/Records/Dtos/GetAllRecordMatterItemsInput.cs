using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Records.Dtos
{
	public class GetAllRecordMatterItemsInput : PagedAndSortedResultRequestDto
	{
		//public string Filter { get; set; }
		//public string DocumentFilter { get; set; }
		//public string DocumentNameFilter { get; set; }
		//public string RecordMatterTenantIdFilter { get; set; }
		public Guid Id { get; set; }
	}
}