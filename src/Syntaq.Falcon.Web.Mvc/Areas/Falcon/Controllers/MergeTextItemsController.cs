using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.MergeTextItems;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.MergeTexts;
using Syntaq.Falcon.MergeTexts.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_MergeTextItems)]
    public class MergeTextItemsController : FalconControllerBase
    {
        private readonly IMergeTextItemsAppService _mergeTextItemsAppService;

        public MergeTextItemsController(IMergeTextItemsAppService mergeTextItemsAppService)
        {
            _mergeTextItemsAppService = mergeTextItemsAppService;
        }

        public ActionResult Index()
        {
            var model = new MergeTextItemsViewModel
			{
				FilterText = ""
			};

            return View(model);
        }

#if STQ_PRODUCTION

        [AbpMvcAuthorize(AppPermissions.Pages_MergeTextItems_Create, AppPermissions.Pages_MergeTextItems_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(long? id)
        {
			GetMergeTextItemForEditOutput getMergeTextItemForEditOutput;

			if (id.HasValue){
				getMergeTextItemForEditOutput = await _mergeTextItemsAppService.GetMergeTextItemForEdit(new EntityDto<long> { Id = (long) id });
			}
			else{
				getMergeTextItemForEditOutput = new GetMergeTextItemForEditOutput{
					MergeTextItem = new CreateOrEditMergeTextItemDto()
				};
			}

            var viewModel = new CreateOrEditMergeTextItemModalViewModel()
            {
				MergeTextItem = getMergeTextItemForEditOutput.MergeTextItem,
					MergeTextItemValueKey = getMergeTextItemForEditOutput.MergeTextItemValueKey
            };

            return PartialView("_CreateOrEditModal", viewModel);
        }


        [AbpMvcAuthorize(AppPermissions.Pages_MergeTextItems_Create, AppPermissions.Pages_MergeTextItems_Edit)]
        public PartialViewResult MergeTextItemValueLookupTableModal(long? id, string displayName)
        {
            var viewModel = new MergeTextItemMergeTextItemValueLookupTableViewModel()
            {
                Id = id,
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_MergeTextItemMergeTextItemValueLookupTableModal", viewModel);
        }

#endif

    }
}