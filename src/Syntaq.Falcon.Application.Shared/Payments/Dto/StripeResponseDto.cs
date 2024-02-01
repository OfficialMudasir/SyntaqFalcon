namespace Syntaq.Falcon.Payments.Dto
{	public class StripeResponseDto
	{
		public string Entity { get; set; }
		public string Key { get; set; }
		public long? UserId { get; set; }
		public long? TenantId { get; set; }
	}
}