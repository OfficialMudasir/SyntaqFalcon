using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Folders;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Folders;
using Syntaq.Falcon.Folders.Dtos;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
	[Area("Falcon")]
	[AbpMvcAuthorize(AppPermissions.Pages_Folders)]
	public class FoldersController : FalconControllerBase
	{
		private readonly IFoldersAppService _foldersAppService;

		public FoldersController(IFoldersAppService foldersAppService)
		{
			_foldersAppService = foldersAppService;
		}

		public ActionResult Index()
		{
			var model = new FoldersViewModel
			{
				FilterText = ""
			};

			return View(model);
		}

		[AbpMvcAuthorize(AppPermissions.Pages_Folders_Create, AppPermissions.Pages_Folders_Edit)]
		public async Task<PartialViewResult> CreateOrEditModal(Guid? Id, string ParentId, string Type)
		{
			GetFolderForEditOutput getFolderForEditOutput;

			if (Id.HasValue){
				getFolderForEditOutput = await _foldersAppService.GetFolderForEdit(new EntityDto<Guid> { Id = (Guid)Id });
			}
			else{
				getFolderForEditOutput = new GetFolderForEditOutput {
					Folder = new CreateOrEditFolderDto()
					{
						ParentId = Guid.Parse(ParentId),
						Type = Type
					}
				};
			}

			var viewModel = new CreateOrEditFolderModalViewModel()
			{
				Folder = getFolderForEditOutput.Folder
			};

			return PartialView("_CreateOrEditModal", viewModel);
		}

		[AbpMvcAuthorize(AppPermissions.Pages_RecordMatters_Create, AppPermissions.Pages_RecordMatters_Edit)]
		public PartialViewResult FoldersLookupTableModal(Guid? id, string displayName)
		{
			var viewModel = new FolderLookupTableViewModel()
			{
				Id = id.ToString(),
				DisplayName = displayName,
				FilterText = ""
			};

			return PartialView("_FoldersLookupTableModal", viewModel);
		}

	}
}