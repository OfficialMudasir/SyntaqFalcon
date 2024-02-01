using System;

namespace Syntaq.Falcon.Payments.Dto
{
	public class StripeChargeDto
	{
		public Guid FormId { get; set; }
		public Guid SubmissionId { get; set; }
		public string Token { get; set; }
        public string VoucherKey { get; set; }
        public string FormData { get; set; } // used for payment rules preocessing
    }

    public class StGeorgeChargeDto
    {

        public string FormId { get; set; }
        public string SubmissionId { get; set; }

        public string CardYear { get; set; }
        public string CardMonth { get; set; }
        public string CardNumber { get; set; }
        public string CardAmount { get; set; }
        public string CardCVC2 { get; set; }

        public string ClientRef { get; set; }
        public string VoucherKey { get; set; }
        public string FormData { get; set; } // used for payment rules preocessing
    }


    public class StGeorgeCharge
    {

        // CREDITCARD
        public string @interface
        {
            get {return "CREDITCARD";}
        }  

        // LC Client ID only used for LC Tenant
        public string clientid { get; set; } = "10011702";

        public string authenticationtoken { get; set; } = "ypzRLpk1sbigZpOA";

        // Credit Card no test no = 4111111111111111
        public string carddata { get; set; } = "4111111111111111";

        // 4 digit i.e 1222
        public string cardexpirydate { get; set; } = "1222";

        public string cvc2 { get; set; } = "132";

        // Decimal i.e. 10.00
        public string totalamount { get; set; } = "10.00";

        public string transactiontype { get
            {
                return "PURCHASE";
            }
        }

    }
}
