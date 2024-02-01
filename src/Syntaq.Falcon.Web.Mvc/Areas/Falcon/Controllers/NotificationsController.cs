using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Notifications;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Notification;
using Syntaq.Falcon.Web.Controllers;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize]
    public class NotificationsController : FalconControllerBase
    {
        private readonly INotificationAppService _notificationApp;

        public NotificationsController(INotificationAppService notificationApp)
        {
            _notificationApp = notificationApp;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<PartialViewResult> SettingsModal()
        {
            var notificationSettings = await _notificationApp.GetNotificationSettings();
            return PartialView("_SettingsModal", notificationSettings);
        }

        public async Task<PartialViewResult> ManageRecipientsModal(string defaultMessage, string entityNames)
        {
            ManageNotificationRecipientsViewModel viewModel = new ManageNotificationRecipientsViewModel()
            {
                DefaultMessage = defaultMessage,
                EntityNames = entityNames
            };
            return PartialView("_ManageRecipientsModal", viewModel);
        }
    }
}