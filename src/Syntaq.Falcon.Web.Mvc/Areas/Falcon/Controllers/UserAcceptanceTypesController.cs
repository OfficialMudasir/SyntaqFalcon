using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.UserAcceptanceTypes;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Users;
using Syntaq.Falcon.Users.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_UserAcceptanceTypes)]
    public class UserAcceptanceTypesController : FalconControllerBase
    {
        private readonly IUserAcceptanceTypesAppService _userAcceptanceTypesAppService;

        public UserAcceptanceTypesController(IUserAcceptanceTypesAppService userAcceptanceTypesAppService)
        {
            _userAcceptanceTypesAppService = userAcceptanceTypesAppService;
        }

        public ActionResult Index()
        {
            var model = new UserAcceptanceTypesViewModel
			{
				FilterText = ""
			};

            return View(model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_UserAcceptanceTypes_Create, AppPermissions.Pages_UserAcceptanceTypes_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(Guid? id)
        {
			GetUserAcceptanceTypeForEditOutput getUserAcceptanceTypeForEditOutput;

			if (id.HasValue){
				getUserAcceptanceTypeForEditOutput = await _userAcceptanceTypesAppService.GetUserAcceptanceTypeForEdit(new EntityDto<Guid> { Id = (Guid) id });
			}
			else{
				getUserAcceptanceTypeForEditOutput = new GetUserAcceptanceTypeForEditOutput{
					UserAcceptanceType = new CreateOrEditUserAcceptanceTypeDto()
				};
			}

            var viewModel = new CreateOrEditUserAcceptanceTypeModalViewModel()
            {
				UserAcceptanceType = getUserAcceptanceTypeForEditOutput.UserAcceptanceType,
					TemplateName = getUserAcceptanceTypeForEditOutput.TemplateName
            };

            return PartialView("_CreateOrEditModal", viewModel);
        }


        [AbpMvcAuthorize(AppPermissions.Pages_UserAcceptanceTypes_Create, AppPermissions.Pages_UserAcceptanceTypes_Edit)]
        public PartialViewResult TemplateLookupTableModal(Guid? id, string displayName)
        {
            var viewModel = new TemplateLookupTableViewModel()
            {
                Id = id.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_TemplateLookupTableModal", viewModel);
        }

    }
}