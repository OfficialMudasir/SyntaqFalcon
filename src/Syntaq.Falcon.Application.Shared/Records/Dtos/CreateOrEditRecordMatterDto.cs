using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Records.Dtos
{
	public class CreateOrEditRecordMatterDto : FullAuditedEntityDto<Guid?>
	{
		public string RecordMatterName { get; set; }
		[StringLength(RecordMatterConsts.MaxCommentsLength, MinimumLength = RecordMatterConsts.MinCommentsLength)]
		public string Comments { get; set; }

		public string Data { get; set; }
				
		[StringLength(RecordMatterConsts.MaxAccessTokenLength, MinimumLength = RecordMatterConsts.MinAccessTokenLength)]
		public string AccessToken { get; set; }
				
		public Guid RecordId { get; set; }

		public long? UserId { get; set; }

		public long? OrganizationUnitId { get; set; }
	}
}