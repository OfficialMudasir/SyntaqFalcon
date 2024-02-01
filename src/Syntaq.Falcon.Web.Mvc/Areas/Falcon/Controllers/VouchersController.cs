using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Vouchers;
using Syntaq.Falcon.Vouchers.Dtos;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Vouchers;
using Syntaq.Falcon.Web.Controllers;
using System;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_Vouchers)]
    public class VouchersController : FalconControllerBase
    {
        private readonly IVouchersAppService _vouchersAppService;

        public VouchersController(IVouchersAppService vouchersAppService)
        {
            _vouchersAppService = vouchersAppService;
        }

        public ActionResult Index()
        {
            var model = new VouchersViewModel
			{
				FilterText = ""
			};

            return View(model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Vouchers_Create, AppPermissions.Pages_Vouchers_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(Guid? id)
        {
			GetVoucherForEditOutput getVoucherForEditOutput;

			if (id.HasValue){
				getVoucherForEditOutput = await _vouchersAppService.GetVoucherForEdit(new EntityDto<Guid> { Id = (Guid) id });
			}
			else
            {
				getVoucherForEditOutput = new GetVoucherForEditOutput
                {
					Voucher = new CreateOrEditVoucherDto()
				};
				getVoucherForEditOutput.Voucher.Expiry = DateTime.Now.AddDays(30);

                string vKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);

                var result = false;
                while (result == false)
                {
                    result = await _vouchersAppService.CheckVoucherKeyNotExisting(vKey);

                    if (result != false)
                    {
                        getVoucherForEditOutput.Voucher.Key = vKey;
                    }
                    else
                    {
                        vKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
                    }
                }
            }

            var viewModel = new CreateOrEditVoucherModalViewModel()
            {
				Voucher = getVoucherForEditOutput.Voucher
            };
            return PartialView("_CreateOrEditModal", viewModel);
        }

        public async Task<PartialViewResult> ViewVoucherModal(Guid id)
        {
			var getVoucherForViewDto = await _vouchersAppService.GetVoucherForView(id);

            var model = new VoucherViewModel()
            {
				Voucher = getVoucherForViewDto.Voucher

            };

            return PartialView("_ViewVoucherModal", model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Vouchers_Create, AppPermissions.Pages_Vouchers_Edit)]
        public PartialViewResult EntityLookupTableModal(Guid? id, string displayName)
        {
            var viewModel = new EntityLookupTableViewModel() 
            {
                Id = id.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };
            return PartialView("_EntityLookupTableModal", viewModel);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Vouchers_Create, AppPermissions.Pages_Vouchers_Edit)]
        public PartialViewResult ViewVoucherUsageLookupTableModal(Guid id)
        {
            var viewModel = new VoucherUsageLookupViewModel()
            {
                VoucherId = id
            };
            return PartialView("_ViewVoucherUsageLookupTableModal", viewModel);
        }
    }
}