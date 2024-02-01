using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Web.Areas.Falcon.Models.RecordMatters;
using Syntaq.Falcon.Web.Controllers;
using System;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
	[Area("Falcon")]
	[AbpMvcAuthorize(AppPermissions.Pages_RecordMatters)]
	public class RecordMattersController : FalconControllerBase
	{
		private readonly IRecordMattersAppService _recordMattersAppService;

		public RecordMattersController(IRecordMattersAppService recordMattersAppService)
		{
			_recordMattersAppService = recordMattersAppService;
		}

		//public ActionResult Index()
		//{
		//	var model = new RecordMattersViewModel
		//	{
		//		FilterText = ""
		//	};

		//	return View(model);
		//}

		[AbpMvcAuthorize(AppPermissions.Pages_RecordMatters_Create, AppPermissions.Pages_RecordMatters_Edit)]
		[Authorize(Policy = "EditById")]
		public async Task<PartialViewResult> CreateOrEditModal(Guid? id, string RecordId)
		{
			GetRecordMatterForEditOutput getRecordMatterForEditOutput;
			if (id.HasValue)
			{
				//getRecordMatterForEditOutput = await _recordMattersAppService.GetRecordMatterForEdit(new EntityDto<Guid> { Id = (Guid)id });
                getRecordMatterForEditOutput = await _recordMattersAppService.GetRecordMatterForEdit((Guid)id , new Guid(), null, string.Empty);
            }
			else
			{
				getRecordMatterForEditOutput = new GetRecordMatterForEditOutput() { RecordMatter = new CreateOrEditRecordMatterDto() { RecordId = Guid.Parse(RecordId) } };
			}
			var viewModel = new CreateOrEditRecordMatterModalViewModel()
			{
				RecordMatter = getRecordMatterForEditOutput.RecordMatter				
			};
			return PartialView("_CreateOrEditModal", viewModel);
		}

		[AbpMvcAuthorize(AppPermissions.Pages_RecordMatters_Create, AppPermissions.Pages_RecordMatters_Edit)]
		//Needs Authorisation? Is method used?
		public PartialViewResult RecordLookupTableModal(Guid? id, string displayName)
		{
			var viewModel = new RecordLookupTableViewModel()
			{
				Id = id.ToString(),
				DisplayName = displayName,
				FilterText = ""
			};
			return PartialView("_RecordLookupTableModal", viewModel);
		}
	}
}