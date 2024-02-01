using System.Threading.Tasks;
using Abp.Application.Services;
using Syntaq.Falcon.MultiTenancy.Payments.Dto;
using Syntaq.Falcon.MultiTenancy.Payments.Stripe.Dto;

namespace Syntaq.Falcon.MultiTenancy.Payments.Stripe
{
    public interface IStripePaymentAppService : IApplicationService
    {
        Task ConfirmPayment(StripeConfirmPaymentInput input);

        StripeConfigurationDto GetConfiguration();

        Task<SubscriptionPaymentDto> GetPaymentAsync(StripeGetPaymentInput input);

        Task<string> CreatePaymentSession(StripeCreatePaymentSessionInput input);
    }
}