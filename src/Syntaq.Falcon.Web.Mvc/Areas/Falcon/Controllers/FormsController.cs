using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Folders;
using Syntaq.Falcon.Folders.Dtos;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Forms.Dtos;
using Syntaq.Falcon.MultiTenancy;
using Syntaq.Falcon.Projects;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Sessions;
using Syntaq.Falcon.Submissions;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Forms;
using Syntaq.Falcon.Web.Areas.Falcon.Startup;
using Syntaq.Falcon.Web.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
	[Area("Falcon")]
	[AllowAnonymous]
	public class FormsController : FalconControllerBase
	{
		private readonly SubmissionManager _submissionManager;
		private readonly IFormsAppService _formsAppService;
		private readonly IFoldersAppService _foldersAppService;
		private readonly IFormRulesAppService _formRulesAppService;
		private readonly IRecordsAppService _recordAppService;
		private readonly IRecordMattersAppService _recordMattersAppService;
		private readonly IRecordMatterItemsAppService _recordMatterItemsAppService; 
		private readonly ISessionAppService _sessionAppService;

		private readonly TenantManager _tenantManager;
		private readonly IRepository<Project, Guid> _projectRepository;
		private readonly IRepository<RecordMatter, Guid> _recordMatterRepository;

		public FormsController(
			SubmissionManager submissionManager, 
			IFormsAppService formsAppService, 
			IFoldersAppService foldersAppService, 
			IFormRulesAppService formRulesAppService, 
			IRecordsAppService recordAppService, 
			IRecordMattersAppService recordMattersAppService,
			TenantManager tenantManager,
			IRecordMatterItemsAppService recordMatterItemsAppService, 
			ISessionAppService SessionAppService,
			IRepository<Project, Guid> projectRepository,
			IRepository<RecordMatter, Guid> recordMatterRepository)
		{
			_submissionManager = submissionManager;
			_formsAppService = formsAppService;
			_foldersAppService = foldersAppService;
			_formRulesAppService = formRulesAppService;
			_recordAppService = recordAppService;
			_recordMattersAppService = recordMattersAppService;
			_recordMatterItemsAppService = recordMatterItemsAppService;
			_sessionAppService = SessionAppService;
			_tenantManager = tenantManager;
			_projectRepository = projectRepository;
			_recordMatterRepository = recordMatterRepository;
		}

	public ActionResult Index()
		{
			var model = new FormsViewModel
			{
				FilterText = ""
			};

            return View(model);
		}

		public async Task<PartialViewResult> FormsPartial(string Id)
		{
			List<FolderDto> currentFoldersTree = await _foldersAppService.GetBreadcrumbs(Id, "F");
			var totalBreadcrumbCount = currentFoldersTree.Count();
			var model = new FormsPartialViewModel
			{
				FilterText = "",
				BreadcrumbList = currentFoldersTree,
				TotalBreadcrumbCount = totalBreadcrumbCount,
				CurrentFolder = Id != "00000000-0000-0000-0000-000000000000" ? Id : currentFoldersTree[0].Id.ToString()
			};
			return PartialView("_FormsPartial", model);
		}

		public async Task<PartialViewResult> CreateVersionModal(Guid Id, Guid OriginalId, int Version, string Name)
		{
			var model = new GetFormForEditOutput
			{
				Form = new CreateOrEditFormDto()
				{
					Id = Id,
					OriginalId = OriginalId,
					Version = Version,
					Name = Name
				}
			};
			return PartialView("_CreateVersionModal", model);
		}
		// { id: $('#id').val(), "OriginalId": OriginalId, "Version": Version, "Name": VersionName }
		public async Task<PartialViewResult> SetAliveModal(Guid Id, Guid OriginalId, int Version, string Name, string VersionName)
		{
			var model = new GetFormForEditOutput
			{
				Form = new CreateOrEditFormDto()
				{
					Id = Id,
					OriginalId = OriginalId,
					Version = Version,
					Name = Name,
					VersionName = VersionName
				}
			};
			return PartialView("_SetAliveModal", model);
		}

		public async Task<PartialViewResult> FormSettingsModal(Guid? OriginalId, string Version)
		{
			GetFormForEditOutput _Form = await _formsAppService.GetFormForEdit(new GetFormForViewDto() { OriginalId = OriginalId, Version = Version });
			List<FormListDto> formsList = _formsAppService.GetFormsList("normal", "live");
			var model = new FormSettingsViewModel
            {
                FormId = _Form.Form.Id,
                FormName = _Form.Form.Name,
                FormVersionName = _Form.Form.VersionName,
                FormDescription = _Form.Form.Description,
                IsEnabled = _Form.Form.IsEnabled,
                //IsPaymentProviderSet = _Form , Going to need to do some kind of lookup
                IsPaymentEnabled = _Form.Form.PaymentEnabled,
                LockOnBuild = _Form.Form.LockOnBuild,
                LockToTenant = _Form.Form.LockToTenant,
                RequireAuth = _Form.Form.RequireAuth,
				PaymentAmount = _Form.Form.PaymentAmount,
				PaymentCurrency = _Form.Form.PaymentCurrency,
				PaymentProcess = _Form.Form.PaymentProcess,
				FormsList = formsList
			};
			return PartialView("_FormSettingsModal", model);
		}

		[AbpMvcAuthorize(AppPermissions.Pages_Forms_Create, AppPermissions.Pages_Forms_Edit)]
		[Authorize(Policy = "EditById")]
		public async Task<PartialViewResult> CreateOrEditModal(Guid? OriginalId, string Version, string FolderId)
		{
			GetFormForEditOutput getFormForEditOutput;
			var FormType = "";
			if (OriginalId.HasValue)
			{
				getFormForEditOutput = await _formsAppService.GetFormForEdit(new GetFormForViewDto(){ OriginalId = OriginalId, Version = Version });
				JObject form = JObject.Parse(getFormForEditOutput.Form.Schema);
				FormType = form["display"].ToString() == "form" ? "Form" : "Wizard";
			}
			else
			{
				Guid Id = Guid.NewGuid();
				getFormForEditOutput = new GetFormForEditOutput{
					Form = new CreateOrEditFormDto()
					{
						Id = Id,
						OriginalId = Id,
						Version = 1,
						CurrentVersion = 1,
						FolderId = Guid.Parse(FolderId)
					}
				};
			}

			var viewModel = new CreateOrEditFormModalViewModel()
			{
				Type = FormType,
				Form = getFormForEditOutput.Form,
				VersionHistory = await _formsAppService.GetVersionHistory(new EntityDto<Guid>(getFormForEditOutput.Form.OriginalId))
			};
			return PartialView("_CreateOrEditModal", viewModel);
		}

		[Authorize(Policy = "EditByOriginalId")]
		public async Task<ActionResult> RulesEditor(Guid? OriginalId, string Version)
		{
			GetFormForEditOutput getFormForEdit = new GetFormForEditOutput();
			if (OriginalId.HasValue)
			{
				getFormForEdit = (await _formsAppService.GetFormForEdit(new GetFormForViewDto { OriginalId = OriginalId, Version = Version }));
			}
			return View(getFormForEdit);
		}

		[AllowAnonymous]
		public async Task<ActionResult> ScriptEditor(Guid? OriginalId, string Version)
		{
			GetFormForEditOutput getFormForEdit = new GetFormForEditOutput();
			if (OriginalId.HasValue)
			{
				getFormForEdit.Form = (await _formsAppService.GetFormForEdit(new GetFormForViewDto { OriginalId = OriginalId, Version = Version })).Form;
			}
			return View(getFormForEdit);
		}

		[AbpMvcAuthorize(AppPermissions.Pages_Forms_Create, AppPermissions.Pages_Forms_Edit)]
		[Authorize(Policy = "EditByOriginalId")]
		public async Task<ActionResult> Build(Guid? OriginalId, string Version = "live")
		{
			GetFormForEditOutput getFormForEditOutput;
			if (OriginalId.HasValue)
			{
				getFormForEditOutput = await _formsAppService.GetFormForEdit(new GetFormForViewDto { OriginalId = OriginalId, Version = Version });
			}
			else
			{
				OriginalId = Guid.NewGuid();
				getFormForEditOutput = new GetFormForEditOutput
				{
					Form = new CreateOrEditFormDto()
					{
						Id = OriginalId,
						OriginalId = (Guid)OriginalId,
						Version = 1,
						CurrentVersion = 1
					}
				};
			}
			var viewModel = new CreateOrEditFormModalViewModel()
			{
				Form = getFormForEditOutput.Form,
				VersionHistory = await _formsAppService.GetVersionHistory(new EntityDto<Guid>(getFormForEditOutput.Form.OriginalId))
			};
			return View(viewModel);
		}

        [AllowAnonymous]
        public async Task<ActionResult> Load(Guid? Id, Guid? OriginalId, string VersionName, string Version, Guid? RecordId, Guid? RecordMatterId, Guid? ProjectId, string? finalised, string view = "")
		{
			FormLoadViewModel formLoadViewModel = new FormLoadViewModel();
			if (Id.HasValue || OriginalId.HasValue)
			{
				Id = OriginalId;
				VersionName = "Live";
				GetFormForLoad _Form = await _formsAppService.GetFormLoadView(new GetFormForViewDto { Id = Id, OriginalId = OriginalId, VersionName = VersionName, Version = Version });
				formLoadViewModel = ObjectMapper.Map<FormLoadViewModel>(_Form);
				formLoadViewModel.FormName = _Form.FormName;
				formLoadViewModel.RecordId = RecordId;
				formLoadViewModel.RecordMatterId = RecordMatterId;
				formLoadViewModel.ProjectId = ProjectId;
				formLoadViewModel.TenantId = _Form.TenantId;
			}
			else
			{
				// Throw error // not found
			}

			view = string.IsNullOrEmpty(view) ? "Load" : "Load" + "." + view;
			return View(view, formLoadViewModel);
		}

		[AllowAnonymous]
		public async Task<ActionResult> LoadInProject(Guid? Id, Guid? OriginalId, string VersionName, string Version, Guid? RecordId, Guid? RecordMatterId, Guid? ProjectId, string? finalised, string view = "")
		{
			FormLoadViewModel formLoadViewModel = new FormLoadViewModel();
			if (Id.HasValue || OriginalId.HasValue)
			{
				GetFormForLoad _Form = await _formsAppService.GetFormLoadProjectView(new GetFormForViewDto { Id = Id, OriginalId = OriginalId, VersionName = VersionName, Version = Version }, ProjectId);
				formLoadViewModel = ObjectMapper.Map<FormLoadViewModel>(_Form);
				formLoadViewModel.FormName = _Form.FormName;
				formLoadViewModel.RecordId = RecordId;
				formLoadViewModel.RecordMatterId = RecordMatterId;
				formLoadViewModel.ProjectId = ProjectId;
				formLoadViewModel.TenantId = _Form.TenantId;
			}
			else
			{
				// Throw error // not found
			}

			view = string.IsNullOrEmpty(view) ? "Load" : "Load" + "." + view;
			return View(view, formLoadViewModel);
		}

        [AllowAnonymous]
        public async Task<ActionResult> LoadRelease(Guid projectId, Guid formId, Guid? recordId, Guid? recordMatterId, string? finalised, string view = "")
        {
            FormLoadViewModel formLoadViewModel = new FormLoadViewModel();
 
            GetFormForLoad f = await _formsAppService.GetFormFromReleaseView(projectId, formId);
            formLoadViewModel = ObjectMapper.Map<FormLoadViewModel>(f);
            formLoadViewModel.FormName = f.FormName;
            formLoadViewModel.TenantId = f.TenantId;
            formLoadViewModel.RecordId = recordId;
            formLoadViewModel.RecordMatterId = recordMatterId;
            formLoadViewModel.ProjectId = projectId;

			formLoadViewModel.ReleaseId = f.ReleaseId;

            view = string.IsNullOrEmpty(view) ? "Load" : "Load" + "." + view;
            return View(view, formLoadViewModel);
        }

        [AllowAnonymous]
		public async Task<ActionResult> LoadAnon(Guid? Id, Guid? OriginalId, string VersionName, 
			string Version, Guid? RecordId, Guid? RecordMatterId, string AccessToken, string tenant)
		{
			FormLoadViewModel formLoadViewModel = new FormLoadViewModel();
			if (Id.HasValue || OriginalId.HasValue)
			{
				GetFormForLoad _Form = await
					_formsAppService.GetFormLoadView(new GetFormForViewDto
					{
						Id = Id,
						OriginalId = OriginalId,
						VersionName = VersionName,
						Version = Version,
						RecordMatterId = RecordMatterId
					});
				formLoadViewModel = ObjectMapper.Map<FormLoadViewModel>(_Form);
				formLoadViewModel.FormName = _Form.FormName;
				formLoadViewModel.RecordId = RecordId;
				formLoadViewModel.RecordMatterId = RecordMatterId;
				formLoadViewModel.TenantId = _Form.TenantId;
			}

            if (!string.IsNullOrEmpty(tenant))
			{
				formLoadViewModel.TenantId = await _tenantManager.GetTenantId(tenant);
			}

            return View("Load.Anon", formLoadViewModel);
		}

		[AbpMvcAuthorize(AppPermissions.Pages_Forms_Create, AppPermissions.Pages_Forms_Edit, AppPermissions.Pages_Forms)]
        public async Task<ActionResult> LoadAuth(Guid? Id, Guid? OriginalId, string VersionName, string Version, Guid? RecordId, Guid? RecordMatterId)
        {
            FormLoadViewModel formLoadViewModel = new FormLoadViewModel();
            if (Id.HasValue || OriginalId.HasValue)
            {
                GetFormForLoad _Form = await _formsAppService.GetFormLoadView(new GetFormForViewDto { Id = Id, OriginalId = OriginalId, VersionName = VersionName, Version = Version });

                formLoadViewModel = ObjectMapper.Map<FormLoadViewModel>(_Form);
                formLoadViewModel.RecordId = RecordId;
                formLoadViewModel.RecordMatterId = RecordMatterId;

            }
            else
            {
                // Throw error // not found
            }

			return View("Load", formLoadViewModel);
        }

        // Add Method -> LoadMin()
        // recieve from popup componnent formid
        // method retrieve the access token & tenant name (if any)
        [AllowAnonymous]
        public async Task<ActionResult> LoadMin(Guid? FormID, String ProjectId, string FormData)
		{
			var TenantId = AbpSession.TenantId;
			var TenantName = "";
			var AuthToken = "";
			if (TenantId !=null) {
				var session = await _sessionAppService.GetCurrentLoginInformations();
				TenantName = session.Tenant.TenancyName ?? "";
			}

			Project p = null;

            if (! string.IsNullOrEmpty(ProjectId))
			{
				Guid pId = Guid.Empty;
				Guid.TryParse(ProjectId, out pId);
				if (pId != Guid.Empty)
				{
					p = _projectRepository.Get(pId);
				}
            }
			
			var PopupForm = new GetFormForPopupFormView()
			{
				FormId = FormID,
				TenantName = TenantName,
				SyntaqAuthToken = AuthToken,
				ReleaseId = p?.ReleaseId
			};
			return View("Load.min", PopupForm);
		}

		[Authorize(Policy = "EditByOriginalId")]
		public FileResult GetSchemaForDownload(string Id, int version)
		{
            // Task<string> Schema = _formsAppService.GetSchema(new EntityDto<Guid> { Id = Guid.Parse(Id) });
            Task<string> Schema = _formsAppService.GetSchema(new EntityDto<Guid> { Id = Guid.Parse(Id) }, null, version);
            byte[] bytes = ASCIIEncoding.UTF8.GetBytes(Schema.Result);
			// Downloading the .Json
			return File(bytes, System.Net.Mime.MediaTypeNames.Application.Json, "schemaexport.json");
		}

        [Authorize(Policy = "EditById")]
        public PartialViewResult UploadFormRulesSchemaModal(string Id, string Version)
        {
            UploadFormRulesSchemaViewModel model = new UploadFormRulesSchemaViewModel()
            {
                Id = Id,
                Version = Version
            };
            return PartialView("_UploadFormRulesSchemaModal", model);
        }

        [Authorize(Policy = "EditById")]
        public bool UploadFormRulesSchema(string Id, FileDto input)
        {
            var file = Request.Form.Files.First();
            if (file.Length > 0)
            {
                string str = (new StreamReader(file.OpenReadStream())).ReadToEnd();
                try
                {
                    if (IsValidJson(str))
                    {
                        return _formsAppService.SetRulesSchema(Id, str).Result ? true : false;
                    }
                }
                catch (Exception)
                {
                    // Do something about the exception here
                    return false;
                }
            }
            return false;
        }

        [Authorize(Policy = "EditById")]
		public PartialViewResult ImportFormSchemaModal(string Id, string Version)
		{
			ImportFormSchemaViewModel model = new ImportFormSchemaViewModel()
			{
				Id = Id,
				Version = Version
			};
			return PartialView("_ImportFormSchemaModal", model);
		}

		[Authorize(Policy = "EditById")]
		public bool UploadFormSchema(string Id, FileDto input)
		{
			var file = Request.Form.Files.First();
			if (file.Length > 0)
			{
				string str = (new StreamReader(file.OpenReadStream())).ReadToEnd();
				try
				{
					if (IsValidJson(str))
					{
						return _formsAppService.SetSchema(Id, str).Result ? true : false;
					}
				}
				catch (Exception)
				{
					// Do something about the exception here
					return false;
				}
			}
			return false;
		}

		private static bool IsValidJson(string strInput)
		{
			strInput = strInput.Trim();
			if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
				(strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
			{
				try
				{
					var obj = JToken.Parse(strInput);
					return true;
				}
				catch (JsonReaderException)
				{
					//Exception in parsing json
					//Console.WriteLine(jex.Message);
					return false;
				}
				catch (Exception) //some other exception
				{
					//Console.WriteLine(ex.ToString());
					return false;
				}
			}
			else
			{
				return false;
			}
		}
	}
}