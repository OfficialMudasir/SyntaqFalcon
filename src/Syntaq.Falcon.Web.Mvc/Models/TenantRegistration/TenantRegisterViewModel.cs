using Syntaq.Falcon.Editions;
using Syntaq.Falcon.Editions.Dto;
using Syntaq.Falcon.MultiTenancy.Payments;
using Syntaq.Falcon.Security;
using Syntaq.Falcon.MultiTenancy.Payments.Dto;

namespace Syntaq.Falcon.Web.Models.TenantRegistration
{
    public class TenantRegisterViewModel
    {
        public PasswordComplexitySetting PasswordComplexitySetting { get; set; }

        public int? EditionId { get; set; }

        public SubscriptionStartType? SubscriptionStartType { get; set; }

        public EditionSelectDto Edition { get; set; }

        public EditionPaymentType EditionPaymentType { get; set; }
    }
}
