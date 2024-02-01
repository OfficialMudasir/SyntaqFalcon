using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.RecordMatterItemHistories;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Records.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.UI;
using Syntaq.Falcon.AccessControlList;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_RecordMatterItemHistories)]
    public class RecordMatterItemHistoriesController : FalconControllerBase
    {
        private readonly IRecordMatterItemHistoriesAppService _recordMatterItemHistoriesAppService;


        public RecordMatterItemHistoriesController(IRecordMatterItemHistoriesAppService recordMatterItemHistoriesAppService)
        {
            _recordMatterItemHistoriesAppService = recordMatterItemHistoriesAppService;


        }

        public ActionResult Index()
        {
            var model = new RecordMatterItemHistoriesViewModel
            {
                FilterText = ""
            };

            return View(model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_RecordMatterItemHistories_Create, AppPermissions.Pages_RecordMatterItemHistories_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(Guid? id)
        {
            GetRecordMatterItemHistoryForEditOutput getRecordMatterItemHistoryForEditOutput;

            if (id.HasValue)
            {
                getRecordMatterItemHistoryForEditOutput = await _recordMatterItemHistoriesAppService.GetRecordMatterItemHistoryForEdit(new EntityDto<Guid> { Id = (Guid)id });
            }
            else
            {
                getRecordMatterItemHistoryForEditOutput = new GetRecordMatterItemHistoryForEditOutput
                {
                    RecordMatterItemHistory = new CreateOrEditRecordMatterItemHistoryDto()
                };
            }

            var viewModel = new CreateOrEditRecordMatterItemHistoryModalViewModel()
            {
                RecordMatterItemHistory = getRecordMatterItemHistoryForEditOutput.RecordMatterItemHistory,
                RecordMatterItemDocumentName = getRecordMatterItemHistoryForEditOutput.RecordMatterItemDocumentName,
                FormName = getRecordMatterItemHistoryForEditOutput.FormName,
                SubmissionSubmissionStatus = getRecordMatterItemHistoryForEditOutput.SubmissionSubmissionStatus,

            };

            return PartialView("_CreateOrEditModal", viewModel);
        }

        public async Task<PartialViewResult> ViewRecordMatterItemHistoryModal(Guid? id)
        {
            RecordMatterItemHistoryViewModel model = new RecordMatterItemHistoryViewModel();
            model.RecordMatterItemHistory = new RecordMatterItemHistoryDto();
            model.RecordMatterItemHistory.RecordMatterItemId = id;
            return PartialView("_ViewRecordMatterItemHistoryModal", model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_RecordMatterItemHistories_Create, AppPermissions.Pages_RecordMatterItemHistories_Edit)]
        public PartialViewResult RecordMatterItemLookupTableModal(Guid? id, string displayName)
        {
            var viewModel = new RecordMatterItemHistoryRecordMatterItemLookupTableViewModel()
            {
                Id = id.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_RecordMatterItemHistoryRecordMatterItemLookupTableModal", viewModel);
        }
        [AbpMvcAuthorize(AppPermissions.Pages_RecordMatterItemHistories_Create, AppPermissions.Pages_RecordMatterItemHistories_Edit)]
        public PartialViewResult FormLookupTableModal(Guid? id, string displayName)
        {
            var viewModel = new RecordMatterItemHistoryFormLookupTableViewModel()
            {
                Id = id.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_RecordMatterItemHistoryFormLookupTableModal", viewModel);
        }
        [AbpMvcAuthorize(AppPermissions.Pages_RecordMatterItemHistories_Create, AppPermissions.Pages_RecordMatterItemHistories_Edit)]
        public PartialViewResult SubmissionLookupTableModal(Guid? id, string displayName)
        {
            var viewModel = new RecordMatterItemHistorySubmissionLookupTableViewModel()
            {
                Id = id.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_RecordMatterItemHistorySubmissionLookupTableModal", viewModel);
        }


        public FileResult GetDocumentForDownload(Guid id, string format)
        {
            GetRecordMatterItemForDownload getTemplateForEditOutput = _recordMatterItemHistoriesAppService.GetDocumentForDownload(new EntityDto<Guid> { Id = id },   format);
            if (getTemplateForEditOutput.RecordMatterItem.Type != RecordMatterItemForDownloadType.Disallow)
            {
                byte[] fileBytes = getTemplateForEditOutput.RecordMatterItem.Document;
                string fileName = getTemplateForEditOutput.RecordMatterItem.DocumentName;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else
            {
                throw new UserFriendlyException("You are not authorised to download this file type.");
            }
        }

    }
}