using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Tags;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Tags;
using Syntaq.Falcon.Tags.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_Tags)]
    public class TagsController : FalconControllerBase
    {
        private readonly ITagsAppService _tagsAppService;

        public TagsController(ITagsAppService tagsAppService)
        {
            _tagsAppService = tagsAppService;

        }

        public ActionResult Index()
        {
            var model = new TagsViewModel
            {
                FilterText = ""
            };

            return View(model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_Tags_Create, AppPermissions.Pages_Tags_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(Guid? id)
        {
            GetTagForEditOutput getTagForEditOutput;

            if (id.HasValue)
            {
                getTagForEditOutput = await _tagsAppService.GetTagForEdit(new EntityDto<Guid> { Id = (Guid)id });
            }
            else
            {
                getTagForEditOutput = new GetTagForEditOutput
                {
                    Tag = new CreateOrEditTagDto()
                };
            }

            var viewModel = new CreateOrEditTagModalViewModel()
            {
                Tag = getTagForEditOutput.Tag

            };

            return PartialView("_CreateOrEditModal", viewModel);
        }

        public async Task<PartialViewResult> ViewTagModal(Guid id)
        {
            var getTagForViewDto = await _tagsAppService.GetTagForView(id);

            var model = new TagViewModel()
            {
                Tag = getTagForViewDto.Tag
            };

            return PartialView("_ViewTagModal", model);
        }

    }
}