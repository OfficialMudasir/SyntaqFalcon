using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Records.Dtos
{
    public class CreateOrEditRecordMatterItemHistoryDto : EntityDto<Guid?>
    {

        [StringLength(RecordMatterItemHistoryConsts.MaxDocumentNameLength, MinimumLength = RecordMatterItemHistoryConsts.MinDocumentNameLength)]
        public string DocumentName { get; set; }

        public byte Document { get; set; }

        public Guid GroupId { get; set; }

        [StringLength(RecordMatterItemHistoryConsts.MaxAllowedFormatsLength, MinimumLength = RecordMatterItemHistoryConsts.MinAllowedFormatsLength)]
        public string AllowedFormats { get; set; }

        [StringLength(RecordMatterItemHistoryConsts.MaxStatusLength, MinimumLength = RecordMatterItemHistoryConsts.MinStatusLength)]
        public string Status { get; set; }

        public Guid? RecordMatterItemId { get; set; }

        public Guid? FormId { get; set; }

        public Guid? SubmissionId { get; set; }

    }
}