namespace Syntaq.Falcon.AccessControlList.Dtos
{
    public class GetACLForEditOutput
    {
        public int? ACLId { get; set; }

        public long? UserId { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public long? OrganizationUnitId { get; set; }

        public string OrganizationUnitDisplayName { get; set; }

        public string Role { get; set; }

        public long? TargetTenantId { get; set; }
        public string TargetTenantName { get; set; }
        public bool Accepted { get; set; }
        

    }
}