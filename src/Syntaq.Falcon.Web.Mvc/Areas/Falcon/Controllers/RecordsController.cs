using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Apps;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Folders;
using Syntaq.Falcon.Folders.Dtos;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Records;
using Syntaq.Falcon.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
	[Area("Falcon")]
	[AbpMvcAuthorize(AppPermissions.Pages_Records)]
	public class RecordsController : FalconControllerBase
	{
		private readonly IRecordsAppService _recordsAppService;
		private readonly IFoldersAppService _foldersAppService;
        private readonly IFormsAppService _formsAppService;
		private readonly IAppsAppService _appsAppService;

		public RecordsController(IRecordsAppService recordsAppService, IFoldersAppService foldersAppService, IFormsAppService formsAppService, IAppsAppService appsAppService)
		{
			_recordsAppService = recordsAppService;
			_foldersAppService = foldersAppService;
            _formsAppService = formsAppService;
			_appsAppService = appsAppService;
		}

		public ActionResult Index()
		{
			var model = new RecordsViewModel
			{
				FilterText = ""
			};

			return View(model);
		}

		public async Task<PartialViewResult> RecordsPartial(string Id)
		{
			List<FolderDto> currentFoldersTree = await _foldersAppService.GetBreadcrumbs(Id, "R");

			var totalBreadcrumbCount = currentFoldersTree.Count();

			var model = new RecordsPartialViewModel
			{
				FilterText = "",
				BreadcrumbList = currentFoldersTree,
				TotalBreadcrumbCount = totalBreadcrumbCount,
				CurrentFolder = Id != "00000000-0000-0000-0000-000000000000" ? Id : currentFoldersTree[0].Id.ToString()
			};
			return PartialView("_RecordsPartial", model);
		}

		//needs authorisation
		public async Task<PartialViewResult> EmbeddedRecords(/*Guid Id*/)
		{
			//string json = await _recordsAppService.GetRecordJSONData(Id);
			//var viewModel = new RecordsJSONViewModel()
			//{
			//	Data = json
			//};

			return PartialView("EmbeddedRecords"/*, viewModel*/);
		}

		[AbpMvcAuthorize(AppPermissions.Pages_Records_Create, AppPermissions.Pages_Records_Edit)]
		[Authorize(Policy = "EditById")]
		public async Task<PartialViewResult> CreateOrEditModal(Guid? Id, string FolderId/*, long? OrgId*/)
		{
			CreateOrEditRecordDto createOrEditRecordDto;
			if (Id.HasValue)
			{
				GetRecordForEditOutput getRecordForEditOutput = await _recordsAppService.GetRecordForEdit(new EntityDto<Guid> { Id = (Guid)Id });
				createOrEditRecordDto = getRecordForEditOutput.Record;
			}
			else
			{
				createOrEditRecordDto = new CreateOrEditRecordDto() { RecordName = "", FolderId = Guid.Parse(FolderId )};
			}
			return base.PartialView("_CreateOrEditModal", new CreateOrEditRecordModalViewModel() { Record = createOrEditRecordDto });
		}

		[Authorize(Policy = "ViewById")]
		public async Task<PartialViewResult> ViewRecordsJSONModal(Guid Id)
		{
			return base.PartialView("_ViewRecordsJSONModal", new RecordsJSONViewModel() { Data = await _recordsAppService.GetRecordJSONData(Id) });
		}

        [Authorize(Policy = "ViewById")]
        public async Task<PartialViewResult> NewFormForRecord(Guid Id)
        {
            var viewModal = new NewFormForRecordModal {
                RecordID = Id.ToString(),
                FormsList = _formsAppService.GetFormsList("NewFormForRecord","all")
            };

            return base.PartialView("_NewFormForRecord", viewModal);
        }


		[Authorize(Policy = "ViewById")]
		public async Task<PartialViewResult> NewAppForRecord(Guid Id)
		{
			var viewModal = new NewAppForRecordModal
			{
				RecordID = Id.ToString(),
				Data = await _recordsAppService.GetRecordJSONData(Id),
				AppsList = await _appsAppService.GetAppsList()
			};

			return base.PartialView("_NewAppForRecord", viewModal);
		}

	}
}