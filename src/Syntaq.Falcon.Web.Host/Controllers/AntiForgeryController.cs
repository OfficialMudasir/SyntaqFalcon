using Microsoft.AspNetCore.Antiforgery;

namespace Syntaq.Falcon.Web.Controllers
{
    public class AntiForgeryController : FalconControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
