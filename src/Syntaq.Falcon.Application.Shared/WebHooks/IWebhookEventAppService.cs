using System.Threading.Tasks;
using Abp.Webhooks;

namespace Syntaq.Falcon.WebHooks
{
    public interface IWebhookEventAppService
    {
        Task<WebhookEvent> Get(string id);
    }
}
