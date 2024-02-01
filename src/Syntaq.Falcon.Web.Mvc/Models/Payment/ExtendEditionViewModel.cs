using System.Collections.Generic;
using Syntaq.Falcon.Editions.Dto;
using Syntaq.Falcon.MultiTenancy.Payments;

namespace Syntaq.Falcon.Web.Models.Payment
{
    public class ExtendEditionViewModel
    {
        public EditionSelectDto Edition { get; set; }

        public List<PaymentGatewayModel> PaymentGateways { get; set; }
    }
}