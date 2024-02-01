using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.WebHooks.Dto
{
    public class GetAllSendAttemptsInput : PagedInputDto
    {
        public string SubscriptionId { get; set; }
    }
}
