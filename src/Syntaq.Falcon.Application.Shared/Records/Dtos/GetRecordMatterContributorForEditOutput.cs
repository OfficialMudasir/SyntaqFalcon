using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Records.Dtos
{
    public class GetRecordMatterContributorForEditOutput
    {
		public CreateOrEditRecordMatterContributorDto RecordMatterContributor { get; set; }

		public string RecordMatterRecordMatterName { get; set;}

		public string UserName { get; set;}

		public string FormName { get; set;}

		public string FormSchema { get; set; }

		public string FormScript { get; set; }

		public string FormRules { get; set; }

		public string FormPages { get; set; }

	}
}