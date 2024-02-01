namespace Syntaq.Falcon.Authorization.Accounts.Dto
{

    public class GetTenantIdByNameInput
    {
        // An encrypted text which contains tenantId={value} string
        public string TenantName { get; set; }
    }
}