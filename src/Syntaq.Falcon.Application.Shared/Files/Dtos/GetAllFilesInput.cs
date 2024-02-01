using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Files.Dtos
{
	public class GetAllFilesInput : PagedAndSortedResultRequestDto
	{
		public string Filter { get; set; }
		
		public string RecordRecordNameFilter { get; set; }

		public string RecordMatterRecordMatterNameFilter { get; set; }		 
	}
}