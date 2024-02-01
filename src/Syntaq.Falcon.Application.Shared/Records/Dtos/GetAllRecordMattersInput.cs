using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Records.Dtos
{
	public class GetAllRecordMattersInput : PagedAndSortedResultRequestDto
	{
		//public string Filter { get; set; }

		//public byte? MaxDataFilter { get; set; }

		//public byte? MinDataFilter { get; set; }

		//public string AccessTokenFilter { get; set; }
		
		//public string RecordRecordNameFilter { get; set; }

		public Guid Id { get; set; } 
	}
}