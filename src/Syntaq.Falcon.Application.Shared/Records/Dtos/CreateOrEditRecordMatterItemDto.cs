using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Records.Dtos
{
	public class CreateOrEditRecordMatterItemDto : FullAuditedEntityDto<Guid?>
	{
		//public string Document { get; set; }

		[StringLength(RecordMatterItemConsts.MaxDocumentNameLength, MinimumLength = RecordMatterItemConsts.MinDocumentNameLength)]
		public string DocumentName { get; set; }

		public byte[] Document { get; set; }

		public Guid RecordMatterId { get; set; }

		public Guid FormId { get; set; }

		public Guid GroupId { get; set; }

		public long? UserId { get; set; }

		public long? OrganizationUnitId { get; set; }

		public Guid? SubmissionId { get; set; }

		public String Status { get; set; } = "New";
	}
}
