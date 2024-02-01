using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.EntityVersionHistories;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.EntityVersionHistories;
using Syntaq.Falcon.EntityVersionHistories.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_EntityVersionHistories)]
    public class EntityVersionHistoriesController : FalconControllerBase
    {
        private readonly IEntityVersionHistoriesAppService _entityVersionHistoriesAppService;

        public EntityVersionHistoriesController(IEntityVersionHistoriesAppService entityVersionHistoriesAppService)
        {
            _entityVersionHistoriesAppService = entityVersionHistoriesAppService;

        }

        public ActionResult Index()
        {
            var model = new EntityVersionHistoriesViewModel
            {
                FilterText = ""
            };

            return View(model);
        }

        //[AbpMvcAuthorize(AppPermissions.Pages_EntityVersionHistories_Create, AppPermissions.Pages_EntityVersionHistories_Edit)]
        //public async Task<PartialViewResult> CreateOrEditModal(Guid? id)
        //{
        //    GetEntityVersionHistoryForEditOutput getEntityVersionHistoryForEditOutput;

        //    if (id.HasValue)
        //    {
        //        getEntityVersionHistoryForEditOutput = await _entityVersionHistoriesAppService.GetEntityVersionHistoryForEdit(new EntityDto<Guid> { Id = (Guid)id });
        //    }
        //    else
        //    {
        //        getEntityVersionHistoryForEditOutput = new GetEntityVersionHistoryForEditOutput
        //        {
        //            EntityVersionHistory = new CreateOrEditEntityVersionHistoryDto()
        //        };
        //    }

        //    var viewModel = new CreateOrEditEntityVersionHistoryModalViewModel()
        //    {
        //        EntityVersionHistory = getEntityVersionHistoryForEditOutput.EntityVersionHistory,
        //        UserName = getEntityVersionHistoryForEditOutput.UserName,
        //        EntityVersionHistoryUserList = await _entityVersionHistoriesAppService.GetAllUserForTableDropdown(),

        //    };

        //    return PartialView("_CreateOrEditModal", viewModel);
        //}

        public async Task<PartialViewResult> ViewEntityVersionHistoryModal(Guid id)
        {
            var getEntityVersionHistoryForViewDto = await _entityVersionHistoriesAppService.GetEntityVersionHistoryForView(id);

            var model = new EntityVersionHistoryViewModel()
            {
                EntityVersionHistory = getEntityVersionHistoryForViewDto.EntityVersionHistory
                ,
                UserName = getEntityVersionHistoryForViewDto.UserName

            };

            return PartialView("_ViewEntityVersionHistoryModal", model);
        }

    }
}