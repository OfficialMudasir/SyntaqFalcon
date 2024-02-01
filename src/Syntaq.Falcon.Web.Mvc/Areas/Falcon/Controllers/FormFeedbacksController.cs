using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.FormFeedbacks;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Forms.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_FormFeedbacks)]
    public class FormFeedbacksController : FalconControllerBase
    {
        private readonly IFormFeedbacksAppService _formFeedbacksAppService;

        public FormFeedbacksController(IFormFeedbacksAppService formFeedbacksAppService)
        {
            _formFeedbacksAppService = formFeedbacksAppService;
        }

        public ActionResult Index()
        {
            var model = new FormFeedbacksViewModel
			{
				FilterText = ""
			};

            return View(model);
        } 
       

			 [AbpMvcAuthorize(AppPermissions.Pages_FormFeedbacks_Create, AppPermissions.Pages_FormFeedbacks_Edit)]
			public async Task<PartialViewResult> CreateOrEditModal(Guid? id)
			{
				GetFormFeedbackForEditOutput getFormFeedbackForEditOutput;

				if (id.HasValue){
					getFormFeedbackForEditOutput = await _formFeedbacksAppService.GetFormFeedbackForEdit(new EntityDto<Guid> { Id = (Guid) id });
				}
				else {
					getFormFeedbackForEditOutput = new GetFormFeedbackForEditOutput{
						FormFeedback = new CreateOrEditFormFeedbackDto()
					};
				}

				var viewModel = new CreateOrEditFormFeedbackModalViewModel()
				{
					FormFeedback = getFormFeedbackForEditOutput.FormFeedback,
					FormName = getFormFeedbackForEditOutput.FormName,                
				};

				return PartialView("_CreateOrEditModal", viewModel);
			}
			

        public async Task<PartialViewResult> ViewFormFeedbackModal(Guid id)
        {
			var getFormFeedbackForViewDto = await _formFeedbacksAppService.GetFormFeedbackForView(id);

            var model = new FormFeedbackViewModel()
            {
                FormFeedback = getFormFeedbackForViewDto.FormFeedback,
                FormName = getFormFeedbackForViewDto.FormName,
                UserName = getFormFeedbackForViewDto.UserName,
                Email = getFormFeedbackForViewDto.Email,
                Rating = getFormFeedbackForViewDto.Rating,
                Comment = getFormFeedbackForViewDto.Comment,
                FeedbackFormSchema = getFormFeedbackForViewDto.FeedbackFormSchema,
                FeedbackFormData = getFormFeedbackForViewDto.FeedbackFormData
            };

            return PartialView("_ViewFormFeedbackModal", model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_FormFeedbacks_Create, AppPermissions.Pages_FormFeedbacks_Edit)]
        public PartialViewResult FormLookupTableModal(Guid? id, string displayName)
        {
            var viewModel = new FormFeedbackFormLookupTableViewModel()
            {
                Id = id.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_FormFeedbackFormLookupTableModal", viewModel);
        }

    }
}