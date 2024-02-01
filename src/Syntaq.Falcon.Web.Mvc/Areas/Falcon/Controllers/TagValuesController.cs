using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.TagValues;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Tags;
using Syntaq.Falcon.Tags.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using System.Diagnostics;
using YamlDotNet.Core.Tokens;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_TagValues)]
    public class TagValuesController : FalconControllerBase
    {

        private readonly ITagValuesAppService _tagValuesAppService;

        private readonly ITagsAppService _tagAppService;
        private readonly ITagEntityTypesAppService _tagsEntityTypesAppService;
        private readonly ITagEntitiesAppService _tagsEntitiesAppService;

        public TagValuesController(
            ITagValuesAppService tagValuesAppService,
            ITagsAppService tagAppService,
            ITagEntityTypesAppService tagsEntityTypesAppService,
            ITagEntitiesAppService tagsEntitiesAppService
        )
        {
            _tagValuesAppService = tagValuesAppService;
            _tagAppService = tagAppService;
            _tagsEntityTypesAppService = tagsEntityTypesAppService;
            _tagsEntitiesAppService = tagsEntitiesAppService;
        }

        public ActionResult Index()
        {
            var model = new TagValuesViewModel
            {
                FilterText = ""
            };

            return View(model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_TagValues_Create, AppPermissions.Pages_TagValues_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(Guid? id, string name)
        {
            GetTagValueForEditOutput getTagValueForEditOutput;
            TagHasValue tagHasValue = await _tagValuesAppService.GetTagValueByTagId(new EntityDto<Guid> { Id = (Guid)id });
            bool IfTagHasValue = tagHasValue.ifTagHasValue;

            if(id.HasValue && IfTagHasValue)
            {
                getTagValueForEditOutput = await _tagValuesAppService.GetTagValueForEdit(new EntityDto<Guid> { Id = (Guid)id });
            }
            else
            {

                getTagValueForEditOutput = new GetTagValueForEditOutput
                {
                    TagValue = new CreateOrEditTagValueDto()
                };
                getTagValueForEditOutput.TagValue.TagId = id;
            }

            var viewModel = new CreateOrEditTagValueModalViewModel()
            {
                TagValue = getTagValueForEditOutput.TagValue,
                TagName = id.Value==null ? getTagValueForEditOutput.TagName :name,

            };

            return PartialView("_CreateOrEditModal", viewModel);
        }

        public async Task<PartialViewResult> ViewTagValueModal(Guid id)
        {
            var getTagValueForViewDto = await _tagValuesAppService.GetTagValueForView(id);

            Debug.Write("My error message. 3");

            var model = new TagValueViewModel()
            {
                TagValue = getTagValueForViewDto.TagValue
                ,
                TagName = getTagValueForViewDto.TagName

            };

            return PartialView("_ViewTagValueModal", model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_TagValues_Create, AppPermissions.Pages_TagValues_Edit)]
        public PartialViewResult TagLookupTableModal(Guid? id, string displayName)
        {
            var viewModel = new TagValueTagLookupTableViewModel()
            {
                Id = id.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_TagValueTagLookupTableModal", viewModel);
        }

    }
}