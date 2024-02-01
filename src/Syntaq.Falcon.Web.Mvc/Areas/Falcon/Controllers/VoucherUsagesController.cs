using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.VoucherUsages;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.VoucherUsages;
using Syntaq.Falcon.VoucherUsages.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
	[Area("Falcon")]
	[AbpMvcAuthorize(AppPermissions.Pages_VoucherUsages)]
	public class VoucherUsagesController : FalconControllerBase
	{
		private readonly IVoucherUsagesAppService _voucherUsagesAppService;

		public VoucherUsagesController(IVoucherUsagesAppService voucherUsagesAppService)
		{
			_voucherUsagesAppService = voucherUsagesAppService;
		}

		public ActionResult Index()
		{
			var model = new VoucherUsagesViewModel
			{
				FilterText = ""
			};

			return View(model);
		}

		[AbpMvcAuthorize(AppPermissions.Pages_VoucherUsages_Create, AppPermissions.Pages_VoucherUsages_Edit)]
		public async Task<PartialViewResult> CreateOrEditModal(Guid? id)
		{
			GetVoucherUsageForEditOutput getVoucherUsageForEditOutput;

			if (id.HasValue){
				getVoucherUsageForEditOutput = await _voucherUsagesAppService.GetVoucherUsageForEdit(new EntityDto<Guid> { Id = (Guid) id });
			}
			else{
				getVoucherUsageForEditOutput = new GetVoucherUsageForEditOutput{
					VoucherUsage = new CreateOrEditVoucherUsageDto()
				};
				//getVoucherUsageForEditOutput.VoucherUsage.DateRedeemed = DateTime.Now;
			}

			var viewModel = new CreateOrEditVoucherUsageModalViewModel()
			{
				VoucherUsage = getVoucherUsageForEditOutput.VoucherUsage,
					UserName = getVoucherUsageForEditOutput.UserName
			};

			return PartialView("_CreateOrEditModal", viewModel);
		}

		public async Task<PartialViewResult> ViewVoucherUsageModal(Guid id)
		{
			var getVoucherUsageForViewDto = await _voucherUsagesAppService.GetVoucherUsageForView(id);

			var model = new VoucherUsageViewModel()
			{
				VoucherUsage = getVoucherUsageForViewDto.VoucherUsage
, UserName = getVoucherUsageForViewDto.UserName 

			};

			return PartialView("_ViewVoucherUsageModal", model);
		}

		[AbpMvcAuthorize(AppPermissions.Pages_VoucherUsages_Create, AppPermissions.Pages_VoucherUsages_Edit)]
		public PartialViewResult UserLookupTableModal(long? id, string displayName)
		{
			var viewModel = new UserLookupTableViewModel()
			{
				Id = id,
				DisplayName = displayName,
				FilterText = ""
			};

			return PartialView("_UserLookupTableModal", viewModel);
		}

	}
}