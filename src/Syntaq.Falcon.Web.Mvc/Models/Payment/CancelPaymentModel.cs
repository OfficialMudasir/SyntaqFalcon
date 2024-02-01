using Syntaq.Falcon.MultiTenancy.Payments;

namespace Syntaq.Falcon.Web.Models.Payment
{
    public class CancelPaymentModel
    {
        public string PaymentId { get; set; }

        public SubscriptionPaymentGatewayType Gateway { get; set; }
    }
}