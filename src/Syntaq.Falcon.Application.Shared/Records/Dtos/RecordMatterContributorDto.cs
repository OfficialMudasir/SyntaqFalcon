using Syntaq.Falcon.Projects;

using System;
using Abp.Application.Services.Dto;
using static Syntaq.Falcon.Projects.ProjectConsts;
using Castle.Core;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Records.Dtos
{
	public class RecordMatterContributorDto : EntityDto<Guid>
	{
		public string OrganizationName { get; set; }

		public string Name { get; set; }

		public string AccessToken { get; set; }

		public DateTime Time { get; set; }

		public RecordMatterContributorConsts.RecordMatterContributorStatus Status { get; set; }


		public ProjectStepStatus StepStatus { get; set; }

		public ProjectStepRole StepRole { get; set; }

		public ProjectStepAction StepAction { get; set; }

		public bool Complete { get; set; }

		public string Comments { get; set; }

		public string FormSchema { get; set; }

		public string FormScript { get; set; }

		public string FormRules { get; set; }

		public string FormPages { get; set; }


		public Guid? RecordMatterId { get; set; }

		public long? UserId { get; set; }

		public Guid? FormId { get; set; }

		public bool Enabled {get;set;}



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
		public string ContributorAcceptance { get; set; }

	}
}