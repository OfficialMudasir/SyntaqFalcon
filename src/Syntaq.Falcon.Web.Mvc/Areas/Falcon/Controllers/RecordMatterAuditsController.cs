using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.RecordMatterAudits;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Records.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_RecordMatterAudits)]
    public class RecordMatterAuditsController : FalconControllerBase
    {
        private readonly IRecordMatterAuditsAppService _recordMatterAuditsAppService;

        public RecordMatterAuditsController(IRecordMatterAuditsAppService recordMatterAuditsAppService)
        {
            _recordMatterAuditsAppService = recordMatterAuditsAppService;
        }

        public ActionResult Index()
        {
            var model = new RecordMatterAuditsViewModel
            {
                FilterText = ""
            };

            return View(model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_RecordMatterAudits_Create, AppPermissions.Pages_RecordMatterAudits_Edit)]
        public async Task<ActionResult> CreateOrEdit(Guid? id)
        {
            GetRecordMatterAuditForEditOutput getRecordMatterAuditForEditOutput;

            if (id.HasValue)
            {
                getRecordMatterAuditForEditOutput = await _recordMatterAuditsAppService.GetRecordMatterAuditForEdit(new EntityDto<Guid> { Id = (Guid)id });
            }
            else
            {
                getRecordMatterAuditForEditOutput = new GetRecordMatterAuditForEditOutput
                {
                    RecordMatterAudit = new CreateOrEditRecordMatterAuditDto()
                };
            }

            var viewModel = new CreateOrEditRecordMatterAuditViewModel()
            {
                RecordMatterAudit = getRecordMatterAuditForEditOutput.RecordMatterAudit,
                UserName = getRecordMatterAuditForEditOutput.UserName,
                RecordMatterRecordMatterName = getRecordMatterAuditForEditOutput.RecordMatterRecordMatterName,
            };

            return View(viewModel);
        }

        public async Task<ActionResult> ViewRecordMatterAudit(Guid id)
        {
            var getRecordMatterAuditForViewDto = await _recordMatterAuditsAppService.GetRecordMatterAuditForView(id);

            var model = new RecordMatterAuditViewModel()
            {
                RecordMatterAudit = getRecordMatterAuditForViewDto.RecordMatterAudit
                ,
                UserName = getRecordMatterAuditForViewDto.UserName

                ,
                RecordMatterRecordMatterName = getRecordMatterAuditForViewDto.RecordMatterRecordMatterName

            };

            return View(model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_RecordMatterAudits_Create, AppPermissions.Pages_RecordMatterAudits_Edit)]
        public PartialViewResult UserLookupTableModal(long? id, string displayName)
        {
            var viewModel = new RecordMatterAuditUserLookupTableViewModel()
            {
                Id = id,
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_RecordMatterAuditUserLookupTableModal", viewModel);
        }
        [AbpMvcAuthorize(AppPermissions.Pages_RecordMatterAudits_Create, AppPermissions.Pages_RecordMatterAudits_Edit)]
        public PartialViewResult RecordMatterLookupTableModal(Guid? id, string displayName)
        {
            var viewModel = new RecordMatterAuditRecordMatterLookupTableViewModel()
            {
                Id = id.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_RecordMatterAuditRecordMatterLookupTableModal", viewModel);
        }

    }
}