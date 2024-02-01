using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.MergeTextItemValues;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.MergeTexts;
using Syntaq.Falcon.MergeTexts.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_MergeTextItemValues)]
    public class MergeTextItemValuesController : FalconControllerBase
    {
        private readonly IMergeTextItemValuesAppService _mergeTextItemValuesAppService;

        public MergeTextItemValuesController(IMergeTextItemValuesAppService mergeTextItemValuesAppService)
        {
            _mergeTextItemValuesAppService = mergeTextItemValuesAppService;
        }

        public ActionResult Index()
        {
            var model = new MergeTextItemValuesViewModel
			{
				FilterText = ""
			};

            return View(model);
        }

#if STQ_PRODUCTION

        [AbpMvcAuthorize(AppPermissions.Pages_MergeTextItemValues_Create, AppPermissions.Pages_MergeTextItemValues_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(long? id)
        {
			GetMergeTextItemValueForEditOutput getMergeTextItemValueForEditOutput;

			if (id.HasValue){
				getMergeTextItemValueForEditOutput = await _mergeTextItemValuesAppService.GetMergeTextItemValueForEdit(new EntityDto<long> { Id = (long) id });
			}
			else{
				getMergeTextItemValueForEditOutput = new GetMergeTextItemValueForEditOutput{
					MergeTextItemValue = new CreateOrEditMergeTextItemValueDto()
				};
			}

            var viewModel = new CreateOrEditMergeTextItemValueModalViewModel()
            {
				MergeTextItemValue = getMergeTextItemValueForEditOutput.MergeTextItemValue
            };

            return PartialView("_CreateOrEditModal", viewModel);
        }

#endif

    }
}