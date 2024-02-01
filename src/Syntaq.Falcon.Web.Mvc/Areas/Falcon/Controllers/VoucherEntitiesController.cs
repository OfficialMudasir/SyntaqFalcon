using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.VoucherEntities;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.VoucherEntitites;
using Syntaq.Falcon.VoucherEntitites.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_VoucherEntities)]
    public class VoucherEntitiesController : FalconControllerBase
    {
        private readonly IVoucherEntitiesAppService _voucherEntitiesAppService;

        public VoucherEntitiesController(IVoucherEntitiesAppService voucherEntitiesAppService)
        {
            _voucherEntitiesAppService = voucherEntitiesAppService;
        }

        public ActionResult Index()
        {
            var model = new VoucherEntitiesViewModel
			{
				FilterText = ""
			};

            return View(model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_VoucherEntities_Create, AppPermissions.Pages_VoucherEntities_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(Guid? id)
        {
			GetVoucherEntityForEditOutput getVoucherEntityForEditOutput;

			if (id.HasValue){
				getVoucherEntityForEditOutput = await _voucherEntitiesAppService.GetVoucherEntityForEdit(new EntityDto<Guid> { Id = (Guid) id });
			}
			else{
				getVoucherEntityForEditOutput = new GetVoucherEntityForEditOutput{
					VoucherEntity = new CreateOrEditVoucherEntityDto()
				};
			}

            var viewModel = new CreateOrEditVoucherEntityModalViewModel()
            {
				VoucherEntity = getVoucherEntityForEditOutput.VoucherEntity,
					VoucherTenantId = getVoucherEntityForEditOutput.VoucherTenantId
            };

            return PartialView("_CreateOrEditModal", viewModel);
        }

        public async Task<PartialViewResult> ViewVoucherEntityModal(Guid id)
        {
			var getVoucherEntityForViewDto = await _voucherEntitiesAppService.GetVoucherEntityForView(id);

            var model = new VoucherEntityViewModel()
            {
				VoucherEntity = getVoucherEntityForViewDto.VoucherEntity
, VoucherTenantId = getVoucherEntityForViewDto.VoucherTenantId 

            };

            return PartialView("_ViewVoucherEntityModal", model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_VoucherEntities_Create, AppPermissions.Pages_VoucherEntities_Edit)]
        public PartialViewResult VoucherLookupTableModal(Guid? id, string displayName)
        {
            var viewModel = new VoucherLookupTableViewModel()
            {
                Id = id.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_VoucherLookupTableModal", viewModel);
        }

    }
}