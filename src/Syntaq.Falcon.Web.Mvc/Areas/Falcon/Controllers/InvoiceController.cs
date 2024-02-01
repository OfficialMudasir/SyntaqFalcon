using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.MultiTenancy.Accounting;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Accounting;
using Syntaq.Falcon.Web.Controllers;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    public class InvoiceController : FalconControllerBase
    {
        private readonly IInvoiceAppService _invoiceAppService;

        public InvoiceController(IInvoiceAppService invoiceAppService)
        {
            _invoiceAppService = invoiceAppService;
        }


        [HttpGet]
        public async Task<ActionResult> Index(long paymentId)
        {
            var invoice = await _invoiceAppService.GetInvoiceInfo(new EntityDto<long>(paymentId));
            var model = new InvoiceViewModel
            {
                Invoice = invoice
            };

            return View(model);
        }
    }
}