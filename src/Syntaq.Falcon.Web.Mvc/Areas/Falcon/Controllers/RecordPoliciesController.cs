using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.RecordPolicies;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.RecordPolicies;
using Syntaq.Falcon.RecordPolicies.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using Syntaq.Falcon.Web.Areas.Falcon.Models.RecordPolicyActions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_RecordPolicies)]
    public class RecordPoliciesController : FalconControllerBase
    {
        private readonly IRecordPoliciesAppService _recordPoliciesAppService;
        public RecordPoliciesController(IRecordPoliciesAppService recordPoliciesAppService)
        {
            _recordPoliciesAppService = recordPoliciesAppService;
        }

        public ActionResult Index()
        {
            var model = new RecordPoliciesViewModel
            {
                FilterText = ""
            };

            return View(model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_RecordPolicies_Create, AppPermissions.Pages_RecordPolicies_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(Guid? id)
        {
            GetRecordPolicyForEditOutput getRecordPolicyForEditOutput;

            if (id.HasValue)
            {
                getRecordPolicyForEditOutput = await _recordPoliciesAppService.GetRecordPolicyForEdit(new EntityDto<Guid> { Id = (Guid)id });
            }
            else
            {
                getRecordPolicyForEditOutput = await _recordPoliciesAppService.GetRecordPolicyForCreate();
                //getRecordPolicyForEditOutput = new GetRecordPolicyForEditOutput
                //{

                //    RecordPolicy = new CreateOrEditRecordPolicyDto()
                //};
            }

            var viewModel = new CreateOrEditRecordPolicyModalViewModel()
            {
                RecordPolicy = getRecordPolicyForEditOutput.RecordPolicy,
                TenantList = getRecordPolicyForEditOutput.TenantList,

            };

            return PartialView("_CreateOrEditModal", viewModel);
        }

        public async Task<PartialViewResult> ViewRecordPolicyModal(Guid id)
        {
            var getRecordPolicyForViewDto = await _recordPoliciesAppService.GetRecordPolicyForView(id);

            var model = new RecordPolicyViewModel()
            {
                RecordPolicy = getRecordPolicyForViewDto.RecordPolicy
            };

            return PartialView("_ViewRecordPolicyModal", model);
        }

    }
}