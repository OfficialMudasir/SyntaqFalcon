using System.Collections.Generic;
using Syntaq.Falcon.Editions;
using Syntaq.Falcon.Editions.Dto;
using Syntaq.Falcon.MultiTenancy.Payments;
using Syntaq.Falcon.MultiTenancy.Payments.Dto;

namespace Syntaq.Falcon.Web.Models.Payment
{
    public class BuyEditionViewModel
    {
        public SubscriptionStartType? SubscriptionStartType { get; set; }

        public EditionSelectDto Edition { get; set; }

        public decimal? AdditionalPrice { get; set; }

        public EditionPaymentType EditionPaymentType { get; set; }

        public List<PaymentGatewayModel> PaymentGateways { get; set; }
    }
}
