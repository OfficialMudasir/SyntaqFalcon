using Syntaq.Falcon.Records.Dtos;

using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.RecordMatterItemHistories
{
    public class CreateOrEditRecordMatterItemHistoryModalViewModel
    {
        public CreateOrEditRecordMatterItemHistoryDto RecordMatterItemHistory { get; set; }

        public string RecordMatterItemDocumentName { get; set; }

        public string FormName { get; set; }

        public string SubmissionSubmissionStatus { get; set; }

        public bool IsEditMode => RecordMatterItemHistory.Id.HasValue;
    }
}