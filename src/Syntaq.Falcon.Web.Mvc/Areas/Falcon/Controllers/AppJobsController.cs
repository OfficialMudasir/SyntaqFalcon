using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.AppJobs;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Apps;
using Syntaq.Falcon.Apps.Dtos;
using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
	[Area("Falcon")]
	[AbpMvcAuthorize(AppPermissions.Pages_AppJobs)]
	public class AppJobsController : FalconControllerBase
	{
		private readonly IAppJobsAppService _appJobsAppService;

		public AppJobsController(IAppJobsAppService appJobsAppService)
		{
			_appJobsAppService = appJobsAppService;
		}

		public ActionResult Index()
		{
			var model = new AppJobsViewModel
			{
				FilterText = ""
			};

			return View(model);
		}

		[AbpMvcAuthorize(AppPermissions.Pages_AppJobs_Create, AppPermissions.Pages_AppJobs_Edit)]
		public async Task<PartialViewResult> CreateOrEditModal(Guid? id, Guid? Appid, Guid? Entityid)
		{
			GetAppJobForEditOutput getAppJobForEditOutput;

			if (id.HasValue && id != new Guid("00000000-0000-0000-0000-000000000000")){
				getAppJobForEditOutput = await _appJobsAppService.GetAppJobForEdit(new EntityDto<Guid> { Id = (Guid)id });
			}
			else if (Entityid.HasValue && Entityid != new Guid("00000000-0000-0000-0000-000000000000"))
			{
				getAppJobForEditOutput = await _appJobsAppService.GetAppJobForEdit(new EntityDto<Guid> { Id = (Guid)Entityid });
			}
			else
			{
				getAppJobForEditOutput = new GetAppJobForEditOutput{
					AppId = Appid == null ? new Guid() : (Guid)Appid,
					EntityId = Entityid == null ? new Guid() : (Guid)Entityid,
					AppJob = new CreateOrEditAppJobDto()
					{
						AppId = Appid == null ? new Guid() : (Guid)Appid,
						EntityId = Entityid == null ? new Guid() : (Guid)Entityid,
						Document = new List<CreateOrEditAppJobDocumentDto>(),
						RecordMatter = new List<CreateOrEditAppJobRecordMatterDto>(),
						User = new CreateOrEditAppJobUserDto()
						{
							Email = "",
//                            ID = new Guid(), 
							Name = ""
						},
						WorkFlow = new CreateOrEditAppJobWorkFlowDto()
						{
							AfterAssembly = new List<CreateOrEditAppJobRestDto>(),
							BeforeAssembly = new List<CreateOrEditAppJobRestDto>(),
							Email = new List<CreateOrEditAppJobEmailDto>()
						}/*,
						RedirectURL = ""*/
					}
				};
			}

			var viewModel = new CreateOrEditAppJobModalViewModel()
			{
				AppJob = getAppJobForEditOutput.AppJob,
				AppName = getAppJobForEditOutput.AppName,
				AppJobName = getAppJobForEditOutput.AppJobName,
				Id = getAppJobForEditOutput.Id,
				AppId = getAppJobForEditOutput.AppId == null ? new Guid() : (Guid)getAppJobForEditOutput.AppId,
				EntityId = getAppJobForEditOutput.EntityId == null ? new Guid() : (Guid)getAppJobForEditOutput.EntityId,

			};
	 
			viewModel.AppJob.Document = viewModel.AppJob.Document ?? (viewModel.AppJob.Document = new List<CreateOrEditAppJobDocumentDto>());
			viewModel.AppJob.Form = viewModel.AppJob.Form ?? (viewModel.AppJob.Form = new CreateOrEditAppJobFormDto());
			viewModel.AppJob.RecordMatter = viewModel.AppJob.RecordMatter ?? (viewModel.AppJob.RecordMatter = new List<CreateOrEditAppJobRecordMatterDto>());
			viewModel.AppJob.WorkFlow = viewModel.AppJob.WorkFlow ?? (viewModel.AppJob.WorkFlow = new CreateOrEditAppJobWorkFlowDto()
			{
				AfterAssembly = new List<CreateOrEditAppJobRestDto>(),
				BeforeAssembly = new List<CreateOrEditAppJobRestDto>(),
				Email = new List<CreateOrEditAppJobEmailDto>()
			});

			return PartialView("_CreateOrEditModal", viewModel);
		}

		[AbpMvcAuthorize(AppPermissions.Pages_AppJobs_Create, AppPermissions.Pages_AppJobs_Edit)]
		public async Task<ViewResult> CreateOrEdit(Guid? id, Guid? Appid)
		{
			GetAppJobForEditOutput getAppJobForEditOutput;

			if (id.HasValue && id != new Guid("00000000-0000-0000-0000-000000000000"))
			{
				getAppJobForEditOutput = await _appJobsAppService.GetAppJobForEdit(new EntityDto<Guid> { Id = (Guid)id });
			}
			else
			{
				getAppJobForEditOutput = new GetAppJobForEditOutput
				{
					AppId = (Guid)Appid,
					AppJob = new CreateOrEditAppJobDto()
					{
						AppId = (Guid)Appid,
						Document = new List<CreateOrEditAppJobDocumentDto>(),
						RecordMatter = new List<CreateOrEditAppJobRecordMatterDto>(),
						User = new CreateOrEditAppJobUserDto()
						{
							Email = "",
							//                            ID = new Guid(), 
							Name = ""
						},
						WorkFlow = new CreateOrEditAppJobWorkFlowDto()
						{
							AfterAssembly = new List<CreateOrEditAppJobRestDto>(),
							BeforeAssembly = new List<CreateOrEditAppJobRestDto>(),
							Email = new List<CreateOrEditAppJobEmailDto>()
						},
					}
				};
			}

			var viewModel = new CreateOrEditAppJobModalViewModel()
			{
				AppJob = getAppJobForEditOutput.AppJob,
				AppName = getAppJobForEditOutput.AppName,
				AppJobName = getAppJobForEditOutput.AppJobName,
				Id = getAppJobForEditOutput.Id,
				AppId = getAppJobForEditOutput.AppId,
			};


			viewModel.AppJob.Document = viewModel.AppJob.Document ?? (viewModel.AppJob.Document = new List<CreateOrEditAppJobDocumentDto>());
			viewModel.AppJob.Form = viewModel.AppJob.Form ?? (viewModel.AppJob.Form = new CreateOrEditAppJobFormDto());
			viewModel.AppJob.RecordMatter = viewModel.AppJob.RecordMatter ?? (viewModel.AppJob.RecordMatter = new List<CreateOrEditAppJobRecordMatterDto>());
			viewModel.AppJob.WorkFlow = viewModel.AppJob.WorkFlow ?? (viewModel.AppJob.WorkFlow = new CreateOrEditAppJobWorkFlowDto()
			{
				AfterAssembly = new List<CreateOrEditAppJobRestDto>(),
				BeforeAssembly = new List<CreateOrEditAppJobRestDto>(),
				Email = new List<CreateOrEditAppJobEmailDto>()
			});


			return View("_CreateOrEdit", viewModel);
	 
		}

		public PartialViewResult ViewAppJobModal(GetAppJobForView input)
		{
			var model = new AppJobViewModel()
			{
				AppJob = input.AppJob,
				AppName = input.AppName 

			};

			return PartialView("_ViewAppJobModal", model);
		}


		//public async Task<ActionResult> CreateOrEdit(Guid? id)
		//{

		//    //GetAppForEditOutput getAppForEditOutput;

		//    //if (id.HasValue)
		//    //{
		//    //    getAppForEditOutput = await _appJobsAppService.get(new EntityDto<Guid> { Id = (Guid)id });
		//    //}
		//    //else
		//    //{
		//    //    getAppForEditOutput = new GetAppForEditOutput
		//    //    {
		//    //        App = new CreateOrEditAppDto()
		//    //    };
		//    //}

		//    //var viewModel = new CreateOrEditAppModalViewModel()
		//    //{
		//    //    App = getAppForEditOutput.App
		//    //};

		//    return View();
		//}

		[AbpMvcAuthorize(AppPermissions.Pages_AppJobs_Create, AppPermissions.Pages_AppJobs_Edit)]
		public PartialViewResult AppLookupTableModal(Guid? id, string displayName)
		{
			var viewModel = new AppLookupTableViewModel()
			{
				Id = id.ToString(),
				DisplayName = displayName,
				FilterText = ""
			};
			return PartialView("_AppLookupTableModal", viewModel);
		}
	}
}