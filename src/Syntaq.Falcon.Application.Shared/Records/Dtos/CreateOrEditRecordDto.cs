using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Records.Dtos
{
	public class CreateOrEditRecordDto : FullAuditedEntityDto<Guid?>
	{
		[Required]
		[StringLength(RecordConsts.MaxRecordNameLength, MinimumLength = RecordConsts.MinRecordNameLength)]
		public string RecordName { get; set; }

		[StringLength(RecordConsts.MaxCommentsLength, MinimumLength = RecordConsts.MinCommentsLength)]
		public string Comments { get; set; }

		[Required]
		public Guid FolderId { get; set; }

        //public Guid Id { get; set; }

        public string Data { get; set; }
        
        public long? UserId { get; set; }
        public long? OrganizationUnitId { get; set; }

        [StringLength(RecordMatterConsts.MaxAccessTokenLength, MinimumLength = RecordMatterConsts.MinAccessTokenLength)]
        public string AccessToken { get; set; }

    }
}