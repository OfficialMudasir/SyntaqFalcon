using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.RecordMatterContributors;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Records.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Forms.Dtos;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_RecordMatterContributors)]
    public class RecordMatterContributorsController : FalconControllerBase
    {
        private readonly IRecordMatterContributorsAppService _recordMatterContributorsAppService;
        private readonly IFormsAppService _formsAppService;

        public RecordMatterContributorsController(
            IRecordMatterContributorsAppService recordMatterContributorsAppService,
            IFormsAppService formsAppService
        )
        {
            _recordMatterContributorsAppService = recordMatterContributorsAppService;
            _formsAppService = formsAppService;

        }

        public ActionResult Index()
        {
            var model = new RecordMatterContributorsViewModel
			{
				FilterText = ""
			};

            return View(model);
        } 
       

		[AbpMvcAuthorize(AppPermissions.Pages_RecordMatterContributors_Create, AppPermissions.Pages_RecordMatterContributors_Edit)]
		public async Task<PartialViewResult> CreateOrEditModal(Guid? id, Guid? recordmatterId, Guid? formId, int role)
		{
			GetRecordMatterContributorForEditOutput getRecordMatterContributorForEditOutput;

			if (id.HasValue){
				getRecordMatterContributorForEditOutput = await _recordMatterContributorsAppService.GetRecordMatterContributorForEdit(new EntityDto<Guid> { Id = (Guid) id}, new EntityDto<Guid> { Id = (Guid)recordmatterId }, new EntityDto<Guid> { Id = (Guid)formId });
			}
			else {
				getRecordMatterContributorForEditOutput = new GetRecordMatterContributorForEditOutput{
					RecordMatterContributor = new CreateOrEditRecordMatterContributorDto()
				};
			    getRecordMatterContributorForEditOutput.RecordMatterContributor.Time = DateTime.Now;

                GetFormForEditOutput _form = await _formsAppService.GetFormForAuthor(new GetFormForViewDto() { OriginalId = formId  });

                getRecordMatterContributorForEditOutput.FormName = _form.Form.Name;
                getRecordMatterContributorForEditOutput.FormSchema = _form.Form.Schema;
                getRecordMatterContributorForEditOutput.FormRules = _form.Form.Rules;
                getRecordMatterContributorForEditOutput.FormScript = _form.Form.Script;

                getRecordMatterContributorForEditOutput.RecordMatterContributor.RecordMatterId = recordmatterId;

            }



			var viewModel = new CreateOrEditRecordMatterContributorModalViewModel()
			{
				RecordMatterContributor = getRecordMatterContributorForEditOutput.RecordMatterContributor,
				RecordMatterRecordMatterName = getRecordMatterContributorForEditOutput.RecordMatterRecordMatterName,
				UserName = getRecordMatterContributorForEditOutput.UserName,
				FormName = getRecordMatterContributorForEditOutput.FormName,
            };

            viewModel.RecordMatterContributor.FormId = formId;
            viewModel.RecordMatterContributor.FormPages = getRecordMatterContributorForEditOutput.FormPages;
            viewModel.RecordMatterContributor.FormRules = getRecordMatterContributorForEditOutput.FormRules;
            viewModel.RecordMatterContributor.FormSchema = getRecordMatterContributorForEditOutput.FormSchema;
            viewModel.RecordMatterContributor.FormScript = getRecordMatterContributorForEditOutput.FormScript;
            viewModel.RecordMatterContributor.StepRole = (Projects.ProjectConsts.ProjectStepRole)role;

            return PartialView("_CreateOrEditModal", viewModel);
		}
			

        public async Task<PartialViewResult> ViewRecordMatterContributorModal(Guid id)
        {
			var getRecordMatterContributorForViewDto = await _recordMatterContributorsAppService.GetRecordMatterContributorForView(id);

            var model = new RecordMatterContributorViewModel()
            {
                RecordMatterContributor = getRecordMatterContributorForViewDto.RecordMatterContributor
                , RecordMatterRecordMatterName = getRecordMatterContributorForViewDto.RecordMatterRecordMatterName 

                , UserName = getRecordMatterContributorForViewDto.UserName 

                , FormName = getRecordMatterContributorForViewDto.FormName 

            };

            return PartialView("_ViewRecordMatterContributorModal", model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_RecordMatterContributors_Create, AppPermissions.Pages_RecordMatterContributors_Edit)]
        public PartialViewResult RecordMatterLookupTableModal(Guid? id, string displayName)
        {
            var viewModel = new RecordMatterContributorRecordMatterLookupTableViewModel()
            {
                Id = id.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_RecordMatterContributorRecordMatterLookupTableModal", viewModel);
        }
        [AbpMvcAuthorize(AppPermissions.Pages_RecordMatterContributors_Create, AppPermissions.Pages_RecordMatterContributors_Edit)]
        public PartialViewResult UserLookupTableModal(long? id, string displayName)
        {
            var viewModel = new RecordMatterContributorUserLookupTableViewModel()
            {
                Id = id,
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_RecordMatterContributorUserLookupTableModal", viewModel);
        }
        [AbpMvcAuthorize(AppPermissions.Pages_RecordMatterContributors_Create, AppPermissions.Pages_RecordMatterContributors_Edit)]
        public PartialViewResult FormLookupTableModal(Guid? id, string displayName)
        {
            var viewModel = new RecordMatterContributorFormLookupTableViewModel()
            {
                Id = id.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_RecordMatterContributorFormLookupTableModal", viewModel);
        }


        [AbpMvcAuthorize(AppPermissions.Pages_RecordMatterContributors_Create, AppPermissions.Pages_RecordMatterContributors_Edit)]
        public PartialViewResult RecordMatterContributorCommentModal()
        {
            //var viewModel = new RecordMatterContributorFormLookupTableViewModel()
            //{
            //    Id = id.ToString(),
            //    DisplayName = displayName,
            //    FilterText = ""
            //};

            return PartialView("_RecordMatterContributorCommentModal");
        }

    }
}