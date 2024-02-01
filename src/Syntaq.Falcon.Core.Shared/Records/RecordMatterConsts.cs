namespace Syntaq.Falcon.Records
{
    public class RecordMatterConsts
    {

		public const int MinAccessTokenLength = 0;
		public const int MaxAccessTokenLength = 1024;
        public const int MinCommentsLength = 0;
        public const int MaxCommentsLength = 2048;

        public const int MinStatusLength = 0;
        public const int MaxStatusLength = 32;

        public const int MinRulesSchemaLength = 0;
        public const int MaxRulesSchemaLength = 65536;

        public enum RecordMatterStatus { New, Draft, Share, Final, FinalUnlocked }

    }
}

