namespace Syntaq.Falcon.Records
{
    public class RecordMatterContributorConsts
    {

		public const int MinOrganizationNameLength = 0;
		public const int MaxOrganizationNameLength = 256;
						
		public const int MinNameLength = 0;
		public const int MaxNameLength = 128;
						
		public const int MinAccessTokenLength = 0;
		public const int MaxAccessTokenLength = 2048;
						
		public const int MinEmailLength = 0;
		public const int MaxEmailLength = 512;

		public const int MinSubjectLength = 0;
		public const int MaxSubjectLength = 256;

		public const int MinMessageLength = 0;
		public const int MaxMessageLength = 4096;

		public enum RecordMatterContributorStatus { Awaiting, Rejected, Canceled, Complete, Approved }

	}
}

