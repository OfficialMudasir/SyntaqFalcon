using System.Threading.Tasks;
using Abp.Application.Services;
using Syntaq.Falcon.MultiTenancy.Payments.PayPal.Dto;

namespace Syntaq.Falcon.MultiTenancy.Payments.PayPal
{
    public interface IPayPalPaymentAppService : IApplicationService
    {
        Task ConfirmPayment(long paymentId, string paypalOrderId);

        PayPalConfigurationDto GetConfiguration();
    }
}
