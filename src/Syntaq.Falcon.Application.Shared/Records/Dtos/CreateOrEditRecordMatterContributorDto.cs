using Syntaq.Falcon.Projects;

using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using static Syntaq.Falcon.Projects.ProjectConsts;

namespace Syntaq.Falcon.Records.Dtos
{
    public class CreateOrEditRecordMatterContributorDto : EntityDto<Guid?>
    {

		[StringLength(RecordMatterContributorConsts.MaxOrganizationNameLength, MinimumLength = RecordMatterContributorConsts.MinOrganizationNameLength)]
		public string OrganizationName { get; set; }
		
		
		[StringLength(RecordMatterContributorConsts.MaxNameLength, MinimumLength = RecordMatterContributorConsts.MinNameLength)]
		public string Name { get; set; }
		
		
		[StringLength(RecordMatterContributorConsts.MaxAccessTokenLength, MinimumLength = RecordMatterContributorConsts.MinAccessTokenLength)]
		public string AccessToken { get; set; }
	
		public DateTime Time { get; set; }
		public ProjectStepStatus StepStatus { get; set; }
		public ProjectStepRole StepRole { get; set; }			
		public ProjectStepAction StepAction { get; set; }
				
		public bool Complete { get; set; }

		public string FormSchema { get; set; }				
		public string FormScript { get; set; }				
		public string FormRules { get; set; }				
		public string FormPages { get; set; }
				
		public Guid? RecordMatterId { get; set; }
		public Guid? RecordMatterItemId { get; set; }

		public long? UserId { get; set; }		 
		public Guid? FormId { get; set; }

		[StringLength(RecordMatterContributorConsts.MaxEmailLength, MinimumLength = RecordMatterContributorConsts.MinEmailLength)]
		public string Email { get; set; }

		[StringLength(RecordMatterContributorConsts.MaxEmailLength, MinimumLength = RecordMatterContributorConsts.MinEmailLength)]
		public string EmailFrom { get; set; }

		[StringLength(RecordMatterContributorConsts.MaxEmailLength, MinimumLength = RecordMatterContributorConsts.MinEmailLength)]
		public string EmailCC { get; set; }

		[StringLength(RecordMatterContributorConsts.MaxEmailLength, MinimumLength = RecordMatterContributorConsts.MinEmailLength)]
		public string EmailBCC { get; set; }

		[StringLength(RecordMatterContributorConsts.MaxSubjectLength, MinimumLength = RecordMatterContributorConsts.MinSubjectLength)]
		public string Subject { get; set; }

		[StringLength(RecordMatterContributorConsts.MaxMessageLength, MinimumLength = RecordMatterContributorConsts.MinMessageLength)]
		public string Message { get; set; }
		 
		public bool Enabled { get; set; }

		

	}
}