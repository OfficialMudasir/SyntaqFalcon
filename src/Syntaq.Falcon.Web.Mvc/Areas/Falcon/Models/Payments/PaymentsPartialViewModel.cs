namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Payments
{
	public class PaymentsPartialViewModel
	{
		public string ForView { get; set; }
		public bool HasPaymentConfigured { get; set; }
		public bool IsPaymentEnabled { get; set; }
		public decimal PaymentAmount { get; set; }
		public string PaymentCurrency { get; set; }
		public string PaymentProcess { get; set; }
		public string PaymentAccessToken { get; set; }
		public string PaymentRefreshToken { get; set; }
		public string PaymentPublishableToken { get; set; }
	}
}
