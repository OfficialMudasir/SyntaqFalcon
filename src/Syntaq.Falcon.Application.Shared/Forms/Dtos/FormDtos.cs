using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Forms.Dtos
{
	public class FormFolderDto : EntityDto<Guid>
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public int Version { get; set; }
		public int CurrentVersion { get; set; }
		public Guid OriginalId { get; set; }
		public DateTime LastModified { get; set; }
		public string Type { get; set; }
		public string UserACLPermission { get; set; }

		// USed for Shared Entities from other tenants
		public bool Enabled { get; set; }
		public bool Accepted { get; set; }
		public string TenancyName { get; set; }
		public bool Shared { get; set; }
	}

	public class FormSaveDto
	{
		public string AnonAuthToken { get; set; }
		public string AccessToken { get; set; } // New Access Tokens for contributors will replace AnonAuthToken
		public Guid Id { get; set; }
		public Guid RecordId { get; set; }
		public Guid RecordMatterId { get; set; }
		public Guid RecordMatterItemId { get; set; }
		public Guid SubmissionId { get; set; }
		public dynamic Submission { get; set; }
	}

	public class FormListDto
    {
		public string value { get; set; }
		public string label { get; set; }
		public string Mtext { get; set; }
	}
}