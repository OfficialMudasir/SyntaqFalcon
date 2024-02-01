using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.TagEntityTypes;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Tags;
using Syntaq.Falcon.Tags.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_TagEntityTypes)]
    public class TagEntityTypesController : FalconControllerBase
    {
        private readonly ITagEntityTypesAppService _tagEntityTypesAppService;

        public TagEntityTypesController(ITagEntityTypesAppService tagEntityTypesAppService)
        {
            _tagEntityTypesAppService = tagEntityTypesAppService;

        }

        public ActionResult Index()
        {
            var model = new TagEntityTypesViewModel
            {
                FilterText = ""
            };

            return View(model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_TagEntityTypes_Create, AppPermissions.Pages_TagEntityTypes_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(Guid? id)
        {
            GetTagEntityTypeForEditOutput getTagEntityTypeForEditOutput;

            if (id.HasValue)
            {
                getTagEntityTypeForEditOutput = await _tagEntityTypesAppService.GetTagEntityTypeForEdit(new EntityDto<Guid> { Id = (Guid)id });
            }
            else
            {
                getTagEntityTypeForEditOutput = new GetTagEntityTypeForEditOutput
                {
                    TagEntityType = new CreateOrEditTagEntityTypeDto()
                };
            }

            var viewModel = new CreateOrEditTagEntityTypeModalViewModel()
            {
                TagEntityType = getTagEntityTypeForEditOutput.TagEntityType,
                TagName = getTagEntityTypeForEditOutput.TagName,

            };

            return PartialView("_CreateOrEditModal", viewModel);
        }

        public async Task<PartialViewResult> ViewTagEntityTypeModal(Guid id)
        {
            var getTagEntityTypeForViewDto = await _tagEntityTypesAppService.GetTagEntityTypeForView(id);

            var model = new TagEntityTypeViewModel()
            {
                TagEntityType = getTagEntityTypeForViewDto.TagEntityType
                ,
                TagName = getTagEntityTypeForViewDto.TagName

            };

            return PartialView("_ViewTagEntityTypeModal", model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_TagEntityTypes_Create, AppPermissions.Pages_TagEntityTypes_Edit)]
        public PartialViewResult TagLookupTableModal(Guid? id, string displayName)
        {
            var viewModel = new TagEntityTypeTagLookupTableViewModel()
            {
                Id = id.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_TagEntityTypeTagLookupTableModal", viewModel);
        }

    }
}