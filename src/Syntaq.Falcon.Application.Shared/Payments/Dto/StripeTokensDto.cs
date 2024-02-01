namespace Syntaq.Falcon.Payments.Dto
{
	public class StripeTokensDto
	{
		public StripeResponseDto StripeResponse { get; set; }
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
		public string PublishableKey { get; set; }
	}
}
