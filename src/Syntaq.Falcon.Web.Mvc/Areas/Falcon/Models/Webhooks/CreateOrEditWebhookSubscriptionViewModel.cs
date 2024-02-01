using Abp.Application.Services.Dto;
using Abp.Webhooks;
using Syntaq.Falcon.WebHooks.Dto;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Webhooks
{
    public class CreateOrEditWebhookSubscriptionViewModel
    {
        public WebhookSubscription WebhookSubscription { get; set; }

        public ListResultDto<GetAllAvailableWebhooksOutput> AvailableWebhookEvents { get; set; }
    }
}
