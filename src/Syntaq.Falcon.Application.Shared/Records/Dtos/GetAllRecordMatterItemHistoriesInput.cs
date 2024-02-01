using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Records.Dtos
{
    public class GetAllRecordMatterItemHistoriesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string DocumentNameFilter { get; set; }

        public string AllowedFormatsFilter { get; set; }

        public string StatusFilter { get; set; }

        public string RecordMatterItemDocumentNameFilter { get; set; }

        public string FormNameFilter { get; set; }

        public string SubmissionSubmissionStatusFilter { get; set; }

        public Guid? RecordMatterId { get; set; }

    }
}