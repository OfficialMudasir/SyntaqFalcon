namespace Syntaq.Falcon.Records.Dtos
{
    public class GetRecordMatterItemHistoryForViewDto
    {
        public RecordMatterItemHistoryDto RecordMatterItemHistory { get; set; }

        public string RecordMatterItemDocumentName { get; set; }

        public string FormName { get; set; }

        public string SubmissionSubmissionStatus { get; set; }

    }
}