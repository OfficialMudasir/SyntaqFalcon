using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.RecordPolicyActions;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.RecordPolicyActions;
using Syntaq.Falcon.RecordPolicyActions.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_RecordPolicyActions)]
    public class RecordPolicyActionsController : FalconControllerBase
    {
        private readonly IRecordPolicyActionsAppService _recordPolicyActionsAppService;

        public RecordPolicyActionsController(IRecordPolicyActionsAppService recordPolicyActionsAppService)
        {
            _recordPolicyActionsAppService = recordPolicyActionsAppService;

        }

        public ActionResult Index()
        {
            var model = new RecordPolicyActionsViewModel
            {
                FilterText = ""
            };

            return View(model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_RecordPolicyActions_Create, AppPermissions.Pages_RecordPolicyActions_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(Guid? id, Guid recordPolicyId, int appliedTenantId)
        {
            GetRecordPolicyActionForEditOutput getRecordPolicyActionForEditOutput;

            if (id.HasValue)
            {
                getRecordPolicyActionForEditOutput = await _recordPolicyActionsAppService.GetRecordPolicyActionForEdit(new EntityDto<Guid> { Id = (Guid)id });
            }
            else
            {
                getRecordPolicyActionForEditOutput = new GetRecordPolicyActionForEditOutput
                {
                    RecordPolicyAction = new CreateOrEditRecordPolicyActionDto() { RecordPolicyId = recordPolicyId, AppliedTenantId = appliedTenantId }
                };
            }

            var viewModel = new CreateOrEditRecordPolicyActionModalViewModel()
            {
                RecordPolicyAction = getRecordPolicyActionForEditOutput.RecordPolicyAction,
                RecordPolicyName = getRecordPolicyActionForEditOutput.RecordPolicyName,

            };

            return PartialView("_CreateOrEditModal", viewModel);
        }

        public async Task<PartialViewResult> ViewRecordPolicyActionModal(Guid id)
        {
            var getRecordPolicyActionForViewDto = await _recordPolicyActionsAppService.GetRecordPolicyActionForView(id);

            var model = new RecordPolicyActionViewModel()
            {
                RecordPolicyAction = getRecordPolicyActionForViewDto.RecordPolicyAction,

                AppliedTenantName = getRecordPolicyActionForViewDto.AppliedTenantName

            };

            return PartialView("_ViewRecordPolicyActionModal", model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_RecordPolicyActions_Create, AppPermissions.Pages_RecordPolicyActions_Edit)]
        public PartialViewResult RecordPolicyLookupTableModal(Guid? id, string displayName)
        {
            var viewModel = new RecordPolicyActionRecordPolicyLookupTableViewModel()
            {
                Id = id.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_RecordPolicyActionRecordPolicyLookupTableModal", viewModel);
        }

    }
}