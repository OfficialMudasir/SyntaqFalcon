using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Documents.Dtos
{
	public class GetAllTemplatesInput : PagedAndSortedResultRequestDto
	{
		public string Filter { get; set; }

		public string NameFilter { get; set; }

		public int? MaxVersionFilter { get; set; }
		public int? MinVersionFilter { get; set; }

		public int? MaxCurrentVersionFilter { get; set; }
		public int? MinCurrentVersionFilter { get; set; }

		public string Id { get; set; }
		//public string FolderId { get; set; }

		public string Type { get; set; }

	}
}