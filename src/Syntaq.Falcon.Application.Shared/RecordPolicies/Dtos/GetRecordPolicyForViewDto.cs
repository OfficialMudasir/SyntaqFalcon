namespace Syntaq.Falcon.RecordPolicies.Dtos
{
    public class GetRecordPolicyForViewDto
    {
        public RecordPolicyDto RecordPolicy { get; set; }
        public string AppliedTenantName { get; set; }
    }
}