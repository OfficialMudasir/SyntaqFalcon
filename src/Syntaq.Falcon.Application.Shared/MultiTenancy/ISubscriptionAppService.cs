using System.Threading.Tasks;
using Abp.Application.Services;

namespace Syntaq.Falcon.MultiTenancy
{
    public interface ISubscriptionAppService : IApplicationService
    {
        Task DisableRecurringPayments();

        Task EnableRecurringPayments();
    }
}
