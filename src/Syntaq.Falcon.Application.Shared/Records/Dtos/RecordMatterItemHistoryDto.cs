using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Records.Dtos
{
    public class RecordMatterItemHistoryDto : EntityDto<Guid>
    {
        public string DocumentName { get; set; }
        public string Name { get; set; }

        public string AllowedFormats { get; set; }

        public string Status { get; set; }

        public Guid? RecordMatterItemId { get; set; }

        public Guid? FormId { get; set; }

        public Guid? SubmissionId { get; set; }

        public DateTime CreatedDate { get; set; }

    }
}