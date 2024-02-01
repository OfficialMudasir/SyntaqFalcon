using Abp.Application.Services.Dto;
using Syntaq.Falcon.ProjectTemplates.Dtos;
using System;
using System.Collections.Generic;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class GetAllProjectsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string NameFilter { get; set; }

		public string DescriptionFilter { get; set; }

		public int? StatusFilter { get; set; }

		public int? TypeFilter { get; set; }


		 public string RecordRecordNameFilter { get; set; }

		// Bug will not serialise
		//public List<ProjectTagsDto> Tags { get; set; } = new List<ProjectTagsDto>();

		public string Tags { get; set; }
	}
}