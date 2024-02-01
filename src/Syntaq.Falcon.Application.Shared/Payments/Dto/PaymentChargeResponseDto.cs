using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.Payments.Dto
{
	public class PaymentChargeResponseDto
	{
		public bool PaymentSuccess { get; set; }
		public string PaymentProcess { get; set; }
        public string PaymentMessage { get; set; }
    }
}
