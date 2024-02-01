using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Controllers;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize]
    public class WelcomeController : FalconControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}