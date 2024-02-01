using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.Payments.Dto
{
	public class UpdatePaymentSettingsDto
	{
		public string EntityType { get; set; }
		public string EntityId { get; set; }
		public bool HasPaymentConfigured { get; set; }
		public bool IsPaymentEnabled { get; set; }
		public decimal PaymentAmount { get; set; }
		public string PaymentCurrency { get; set; }
		public string PaymentProcess { get; set; }
		public string PaymentProvider { get; set; }
	}
}
