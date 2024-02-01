namespace Syntaq.Falcon.RecordPolicyActions
{
    public class RecordPolicyActionConsts
    {
        public const int MinNameLength = 1;
        public const int MaxNameLength = 128;

        public enum RecordPolicyActionType { SoftDelete, HardDelete, Archive }
        public enum RecordStatusType { New, InProgress, Completed, Archived, IsDeleted }

    }
}