using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Web.Areas.Falcon.Models.RecordMatterItems;
using Syntaq.Falcon.Web.Controllers;
using System;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
	[Area("Falcon")]
	[AbpMvcAuthorize(AppPermissions.Pages_RecordMatterItems)]
	public class RecordMatterItemsController : FalconControllerBase
	{
		private readonly IRecordMatterItemsAppService _recordMatterItemsAppService;

		public RecordMatterItemsController(IRecordMatterItemsAppService recordMatterItemsAppService)
		{
			_recordMatterItemsAppService = recordMatterItemsAppService;
		}

		//public ActionResult Index()
		//{
		//	var model = new RecordMatterItemsViewModel
		//	{
		//		FilterText = ""
		//	};

		//	return View(model);
		//}

		[AbpMvcAuthorize(AppPermissions.Pages_RecordMatterItems_Create, AppPermissions.Pages_RecordMatterItems_Edit)]
		[Authorize(Policy = "EditById")]
		public async Task<PartialViewResult> CreateOrEditModal(Guid? id)
		{
			GetRecordMatterItemForEditOutput getRecordMatterItemForEditOutput;
			if (id.HasValue)
			{
				getRecordMatterItemForEditOutput = await _recordMatterItemsAppService.GetRecordMatterItemForEdit(new EntityDto<Guid> { Id = (Guid) id });
			}
			else
			{
				getRecordMatterItemForEditOutput = new GetRecordMatterItemForEditOutput{
					RecordMatterItem = new CreateOrEditRecordMatterItemDto()
				};
			}
			var viewModel = new CreateOrEditRecordMatterItemModalViewModel()
			{
				RecordMatterItem = getRecordMatterItemForEditOutput.RecordMatterItem,
				RecordMatterTenantId = getRecordMatterItemForEditOutput.RecordMatterTenantId
			};
			return PartialView("_CreateOrEditModal", viewModel);
		}

		[AbpMvcAuthorize(AppPermissions.Pages_RecordMatterItems_Create, AppPermissions.Pages_RecordMatterItems_Edit)]
		//Needs Authorisation? Is method used?
		public PartialViewResult RecordMatterLookupTableModal(Guid? id, string displayName)
		{
			var viewModel = new RecordMatterLookupTableViewModel()
			{
				Id = id.ToString(),
				DisplayName = displayName,
				FilterText = ""
			};
			return PartialView("_RecordMatterLookupTableModal", viewModel);
		}

		//[Authorize(Policy = "ViewById")]
		[AllowAnonymous]
		public FileResult GetDocumentForDownload(Guid Id, int version, string format, string AccessToken)
		{
			Task<GetRecordMatterItemForDownload> getTemplateForEditOutput = _recordMatterItemsAppService.GetDocumentForDownload(new EntityDto<Guid> { Id = Id }, version, format, AccessToken);
			if (getTemplateForEditOutput.Result.RecordMatterItem.Type != RecordMatterItemForDownloadType.Disallow)
			{
				byte[] fileBytes = getTemplateForEditOutput.Result.RecordMatterItem.Document;
				string fileName = getTemplateForEditOutput.Result.RecordMatterItem.DocumentName;
				return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
			}
			else
			{
				throw new UserFriendlyException("You are not authorised to download this file type.");
			}
		}
	}
}