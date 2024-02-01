using System;

namespace Syntaq.Falcon.Configuration.Host.Dto
{
    public class HostBillingSettingsEditDto
    {
        public string LegalName { get; set; }

        public string Address { get; set; }
    }

    public class HostPaymentFormSettingsEditDto
    {
        public Guid PaymentFormId { get; set; }
        public string StripeClientId { get; set; }
    }

}