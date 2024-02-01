using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Records.Dtos
{
    public class GetAllRecordMatterContributorsForExcelInput
    {
		public string Filter { get; set; }

		public string OrganizationNameFilter { get; set; }

		public string NameFilter { get; set; }

		public string AccessTokenFilter { get; set; }

		public DateTime? MaxTimeFilter { get; set; }
		public DateTime? MinTimeFilter { get; set; }

		public int? StepStatusFilter { get; set; }

		public int? StepRoleFilter { get; set; }

		public int? StepActionFilter { get; set; }

		public int? CompleteFilter { get; set; }

		public string EmailFilter { get; set; }

		public string FormSchemaFilter { get; set; }

		public string FormScriptFilter { get; set; }

		public string FormRulesFilter { get; set; }

		public string FormPagesFilter { get; set; }


		 public string RecordMatterRecordMatterNameFilter { get; set; }

		 		 public string UserNameFilter { get; set; }

		 		 public string FormNameFilter { get; set; }

		 
    }
}