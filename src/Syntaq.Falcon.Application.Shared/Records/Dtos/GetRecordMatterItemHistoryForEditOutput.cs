using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Records.Dtos
{
    public class GetRecordMatterItemHistoryForEditOutput
    {
        public CreateOrEditRecordMatterItemHistoryDto RecordMatterItemHistory { get; set; }

        public string RecordMatterItemDocumentName { get; set; }

        public string FormName { get; set; }

        public string SubmissionSubmissionStatus { get; set; }

    }
}