using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class GetAllProjectsForExcelInput
    {
		public string Filter { get; set; }

		public string NameFilter { get; set; }

		public string DescriptionFilter { get; set; }

		public int? StatusFilter { get; set; }

		public int? TypeFilter { get; set; }


		 public string RecordRecordNameFilter { get; set; }

		 
    }
}