using Abp.Application.Services;
using Syntaq.Falcon.Payments.Dto;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Payments
{
	public interface IPaymentsAppService : IApplicationService
	{
		Task UpdatePaymentSettingsAsync(UpdatePaymentSettingsDto input);

        Task<PaymentChargeResponseDto> ChargeCard(StGeorgeChargeDto stGeorgeCharge);
        Task<PaymentChargeResponseDto> ChargeCardStripe(StripeChargeDto stripeCharge);

    }
}
