using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Apps;
using Syntaq.Falcon.Apps.Dtos;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Apps;
using Syntaq.Falcon.Web.Controllers;
using System;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_Apps)]
    [RequiresFeature("App.AppBuilder")]
    public class AppsController : FalconControllerBase
    {
        private readonly IAppsAppService _appsAppService;

        public AppsController(IAppsAppService appsAppService)
        {
            _appsAppService = appsAppService;
        }

        public ActionResult Index()
        {
            var model = new AppsViewModel
			{
				FilterText = ""
			};

            return View(model);
        }

        public async Task<ActionResult> CreateOrEdit(Guid? id)
        {
            GetAppForEditOutput getAppForEditOutput;

            if (id.HasValue)
            {
                getAppForEditOutput = await _appsAppService.GetAppForEdit(new EntityDto<Guid> { Id = (Guid)id });
            }
            else
            {
                getAppForEditOutput = new GetAppForEditOutput
                {
                    App = new CreateOrEditAppDto()
                };
            }

            var viewModel = new CreateOrEditAppModalViewModel()
            {
                App = getAppForEditOutput.App
            };

            return View(viewModel);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Apps_Create, AppPermissions.Pages_Apps_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(Guid? id)
        {
            GetAppForEditOutput getAppForEditOutput;

			if (id.HasValue){
				getAppForEditOutput = await _appsAppService.GetAppForEdit(new EntityDto<Guid> { Id = (Guid) id });
			}
			else{
				getAppForEditOutput = new GetAppForEditOutput{
					App = new CreateOrEditAppDto()
				};
			}

            var viewModel = new CreateOrEditAppModalViewModel()
            {
				App = getAppForEditOutput.App
            };

            return PartialView("_CreateOrEditModal", viewModel);
        }

        public PartialViewResult ViewAppAsync(GetAppForView data)
        {
            var model = new AppViewModel()
            {
                App = data.App

            };

            return PartialView("_ViewApp", model);
        }

        public PartialViewResult ViewAppModalAsync(GetAppForView data)
        {
            var model = new AppViewModel()
            {
				App = data.App

            };

            return PartialView("_ViewAppModal", model);
        }


    }
}