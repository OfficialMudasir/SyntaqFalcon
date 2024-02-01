namespace Syntaq.Falcon.Web.Models.Account
{
    public class LoginFormViewModel
    {
        public string SuccessMessage { get; set; }
        
        public string UserNameOrEmailAddress { get; set; }

        public bool IsSelfRegistrationEnabled { get; set; }

        public bool IsTenantSelfRegistrationEnabled { get; set; }

        // STQMODIFIED
        public int? TenantId { get; set; }
    }
}