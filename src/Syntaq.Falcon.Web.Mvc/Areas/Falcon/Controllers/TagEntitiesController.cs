using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.TagEntities;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Tags;
using Syntaq.Falcon.Tags.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using Syntaq.Falcon.Web.Areas.Falcon.Models.TagValues;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_TagEntities)]
    public class TagEntitiesController : FalconControllerBase
    {
        private readonly ITagEntitiesAppService _tagEntitiesAppService;
        private readonly ITagEntityTypesAppService _tagEntityTypesAppService;

        public TagEntitiesController(
            ITagEntitiesAppService tagEntitiesAppService, 
            ITagEntityTypesAppService tagEntityTypesAppService            
        )
        {
            _tagEntitiesAppService = tagEntitiesAppService;
            _tagEntityTypesAppService = tagEntityTypesAppService;
        }

        public ActionResult Index()
        {
            var model = new TagEntitiesViewModel
            {
                FilterText = ""
            };

            return View(model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_TagEntities_Create, AppPermissions.Pages_TagEntities_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(Guid? id)
        {
            GetTagEntityForEditOutput getTagEntityForEditOutput;

            if (id.HasValue)
            {
                getTagEntityForEditOutput = await _tagEntitiesAppService.GetTagEntityForEdit(new EntityDto<Guid> { Id = (Guid)id });
            }
            else
            {
                getTagEntityForEditOutput = new GetTagEntityForEditOutput
                {
                    TagEntity = new CreateOrEditTagEntityDto()
                };
            }

            var viewModel = new CreateOrEditTagEntityModalViewModel()
            {
                TagEntity = getTagEntityForEditOutput.TagEntity,
                TagValueValue = getTagEntityForEditOutput.TagValueValue,

            };

            return PartialView("_CreateOrEditModal", viewModel);
        }

        public async Task<PartialViewResult> ViewTagEntityModal(Guid id)
        {
            var getTagEntityForViewDto = await _tagEntitiesAppService.GetTagEntityForView(id);

            var model = new TagEntityViewModel()
            {
                TagEntity = getTagEntityForViewDto.TagEntity
                ,
                TagValueValue = getTagEntityForViewDto.TagValueValue

            };

            return PartialView("_ViewTagEntityModal", model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_TagEntities_Create, AppPermissions.Pages_TagEntities_Edit)]
        public PartialViewResult TagValueLookupTableModal(Guid? id, string displayName)
        {
            var viewModel = new TagEntityTagValueLookupTableViewModel()
            {
                Id = id.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_TagEntityTagValueLookupTableModal", viewModel);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_TagValues_Create, AppPermissions.Pages_TagValues_Edit)]
        public async Task<PartialViewResult> CreateOrEditTagEntitiesModal(Guid id, string name)
        {

            GetTagEntitiesEditOutput getTagValuesEditOutput;

            PagedResultDto<GetTagEntityTypeForViewDto> tags = new PagedResultDto<GetTagEntityTypeForViewDto>();
            PagedResultDto<GetTagEntityForViewDto> entityTags = new PagedResultDto<GetTagEntityForViewDto>();

            // Get the Posible Tags for this Project Template
            GetAllTagEntityTypesInput taginput = new GetAllTagEntityTypesInput()
            {
                EntityTypeFilter = (int)EntityType.Project
            };
            tags = await _tagEntityTypesAppService.GetAll(taginput);

            GetAllTagEntitiesInput tagentityinput = new GetAllTagEntitiesInput()
            {
                EntityIdFilter = id
            };
            entityTags = await _tagEntitiesAppService.GetAll(tagentityinput);

            var viewModel = new CreateOrEditTagEntitiesModalViewModel()
            {
                EntityId = id,
                Tags = tags,
                EntityTags = entityTags,
            };

            return PartialView("_CreateOrEditTagEntitiesModal", viewModel);
        }

    }
}