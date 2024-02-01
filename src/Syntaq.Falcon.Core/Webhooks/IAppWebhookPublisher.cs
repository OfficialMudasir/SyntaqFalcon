using System.Threading.Tasks;
using Syntaq.Falcon.Authorization.Users;

namespace Syntaq.Falcon.WebHooks
{
    public interface IAppWebhookPublisher
    {
        Task PublishTestWebhook();
    }
}
