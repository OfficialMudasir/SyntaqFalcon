using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.IO.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Documents;
using Syntaq.Falcon.Documents.Dtos;
using Syntaq.Falcon.Folders;
using Syntaq.Falcon.Folders.Dtos;
using Syntaq.Falcon.Storage;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Templates;
using Syntaq.Falcon.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
	[Area("Falcon")]
	//[AbpMvcAuthorize(AppPermissions.Pages_Templates)]
	public class TemplatesController : FalconControllerBase
	{
		private readonly ITempFileCacheManager _tempFileCacheManager;
		private const int MaxDocumentTemplateSize = 10485760; //10MB
		private readonly IAppFolders _appFolders;
		private readonly IFoldersAppService _foldersAppService;
		private readonly ITemplatesAppService _templatesAppService;
		private readonly IACLsAppService _ACLsAppService;

		public TemplatesController(ITempFileCacheManager tempFileCacheManager, IAppFolders appFolders, IFoldersAppService foldersAppService, ITemplatesAppService templatesAppService, IACLsAppService ACLsAppService)
		{
			_tempFileCacheManager = tempFileCacheManager;
			_appFolders = appFolders;
			_foldersAppService = foldersAppService;
			_templatesAppService = templatesAppService;
			_ACLsAppService = ACLsAppService;
		}

		public ActionResult Index()
		{
			var model = new TemplatesViewModel
			{
				FilterText = ""
			};
			return View(model);
		}

		public async Task<PartialViewResult> TemplatesPartial(string Id)
		{
			List<FolderDto> currentFoldersTree = await _foldersAppService.GetBreadcrumbs(Id, "T");
			var totalBreadcrumbCount = currentFoldersTree.Count();
			var model = new TemplatesPartialViewModel
			{
				FilterText = "",
				BreadcrumbList = currentFoldersTree,
				TotalBreadcrumbCount = totalBreadcrumbCount,
				CurrentFolder = Id != "00000000-0000-0000-0000-000000000000" ? Id : currentFoldersTree[0].Id.ToString()
			};
			return PartialView("_TemplatesPartial", model);
		}

		[AbpMvcAuthorize(AppPermissions.Pages_Templates_Create, AppPermissions.Pages_Templates_Edit)]
		//[Authorize(Policy = "EditByOriginalId")]
		public async Task<PartialViewResult> CreateOrEditModal(Guid? id, Guid? OriginalId, string FolderId)
		{

			Logger.Info("TemplatesController.CreateOrEditModal.1");

            GetTemplateForEditOutput getTemplateForEditOutput;
			List<GetTemplateForView> getVersionhistoryForView = new List<GetTemplateForView>();

            bool IsEditMode;
			if (OriginalId.HasValue)
			{
				getTemplateForEditOutput = await _templatesAppService.GetTemplateForEdit(new EntityDto<Guid> { Id = (Guid)OriginalId });
				getTemplateForEditOutput.Template.Id = Guid.NewGuid();
				IsEditMode = true;

                Logger.Info("TemplatesController.CreateOrEditModal.2");
                getVersionhistoryForView = await _templatesAppService.GetVersionHistory(getTemplateForEditOutput.Template.OriginalId);
                Logger.Info("TemplatesController.CreateOrEditModal.3");

            }
			else
			{

                Logger.Info("TemplatesController.CreateOrEditModal.4");

                Guid Id = Guid.NewGuid();
				getTemplateForEditOutput = new GetTemplateForEditOutput{
					Template = new CreateOrEditTemplateDto()
					{
						Id = Id,
						OriginalId = Id,
						Version= 1,
						CurrentVersion = 1,
						FolderId = Guid.Parse(FolderId),
						LockToTenant = true
					}
				};
				IsEditMode = false;
			}

            var viewModel = new CreateOrEditTemplateModalViewModel()
			{
				Template = getTemplateForEditOutput.Template,
				VersionHistory = getVersionhistoryForView,
				IsEditMode = IsEditMode
			};

            Logger.Info("TemplatesController.CreateOrEditModal.5");

            return PartialView("_CreateOrEditModal", viewModel);
		}

        //[Authorize(Policy = "EditByOriginalId")]
        public bool UploadDocumentTemplate(Guid? OriginalId)
		{
			try
			{
				var documentTemplateFile = Request.Form.Files.First();
				//Check input
				if (documentTemplateFile == null)
				{
					throw new UserFriendlyException(L("DocumentTemplate_Change_Error"));
				}

				if (documentTemplateFile.Length > MaxDocumentTemplateSize)
				{
					throw new UserFriendlyException(L("DocumentTemplate_Warn_SizeLimit", AppConsts.MaxDocumentTemplateBytesUserFriendlyValue));
				}
				byte[] fileBytes;
				using (var stream = documentTemplateFile.OpenReadStream())
				{
					fileBytes = stream.GetAllBytes();
				}
				_tempFileCacheManager.SetFile(OriginalId.ToString(), fileBytes);
				return true;
			}
			catch (UserFriendlyException)
			{
				return false; // new UploadProfilePictureOutput(new ErrorInfo(ex.Message));
			}
		}

		public async Task<PartialViewResult> SetAliveModal(Guid OriginalId, int Version, int CurrentV, string Name)
		{
			var model = new CreateOrEditTemplateDto
			{
				// Id = Id,
				OriginalId = OriginalId,
				Version = Version,
				CurrentVersion = CurrentV,
				Name = Name
			};
			return PartialView("_SetAliveModal", model);
		}
		//      //[Authorize(Policy = "EditByOriginalId")]
		//      public FileResult GetTemplate(Guid OriginalId, string version = "live")
		//{
		//	Task<GetTemplateForEditOutput> getTemplateForEditOutput =  _templatesAppService.GetTemplateForDownload(OriginalId, version);

		//          if (getTemplateForEditOutput.Result.Template.Document == null)
		//          {
		//              //return   null;// StatusCodes.Status403Forbidden;
		//              //throw new Exception(String.Format("Permission not granted."));
		//              throw new UserFriendlyException("Opps! There is a problem!", "Permission denied.");
		//          }
		//          else
		//          {
		//              byte[] fileBytes = getTemplateForEditOutput.Result.Template.Document;
		//              string fileName = getTemplateForEditOutput.Result.Template.DocumentName;
		//              return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
		//          }

		//}

		public ActionResult GetTemplate(Guid OriginalId, string version = "live")
        {

            Task<GetTemplateForEditOutput> getTemplateForEditOutput = _templatesAppService.GetTemplateForDownload(OriginalId, version);

            if (getTemplateForEditOutput.Result.Template.Document == null)
            {
                //return   null;// StatusCodes.Status403Forbidden;
                //throw new Exception(String.Format("Permission not granted."));
                throw new UserFriendlyException("Opps! There is a problem!", "Permission denied.");
            }
            else
            {
                byte[] fileBytes = getTemplateForEditOutput.Result.Template.Document;
                string fileName = getTemplateForEditOutput.Result.Template.DocumentName;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            //return RedirectToAction("List");
        }

        [AbpMvcAuthorize(AppPermissions.Pages_RecordMatters_Create, AppPermissions.Pages_RecordMatters_Edit)] //RecordMatter Permissions?
		public PartialViewResult TemplateLookupTableModal()
		{
			return PartialView("_TemplateLookupTableModal");
		}
	}
}