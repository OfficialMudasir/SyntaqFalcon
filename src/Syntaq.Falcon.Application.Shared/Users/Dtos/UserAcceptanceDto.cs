
using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Users.Dtos
{
    public class UserAcceptanceDto : EntityDto<Guid>
    {
		// Depreciate this use TemplateVersion 
		// Consistent with UserAcceptanceTypeDto
		public int AcceptedDocTemplateVersion { get; set; } 

		public DateTime CreationTime { get; set; }
		public DateTime? DeletionTime { get; set; }

		public Boolean IsAccepted { get; set; }

		public long? UserId { get; set; }
		public string UserName { get; set; }
		public string UserSurname { get; set; }
		public string UserEmailAddress { get; set; }

		public Guid? RecordMatterContributorId { get; set; }
		public string RecordMatterContributorName { get; set; }

		public Guid? UserAcceptanceTypeId { get; set; }
		public UserAcceptanceTypeDto UserAcceptanceType { get; set; }

		public int TemplateVersion { get; set; }


	}
}