using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Submissions;
using Syntaq.Falcon.Submissions.Dtos;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Submissions;
using Syntaq.Falcon.Web.Controllers;
using System;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
	[Area("Falcon")]
	[AbpMvcAuthorize(AppPermissions.Pages_Submissions)]
	public class SubmissionsController : FalconControllerBase
	{
		private readonly ISubmissionsAppService _submissionsAppService;

		public SubmissionsController(ISubmissionsAppService submissionsAppService)
		{
			_submissionsAppService = submissionsAppService;
		}

		public ActionResult Index()
		{
			var model = new SubmissionsViewModel
			{
				FilterText = ""
			};

			return View(model);
		}

		//[AbpMvcAuthorize(AppPermissions.Pages_Submissions_Create, AppPermissions.Pages_Submissions_Edit)]
		//public async Task<PartialViewResult> CreateOrEditModal(Guid? id)
		//{
		//	GetSubmissionForEditOutput getSubmissionForEditOutput;

		//	if (id.HasValue){
		//		getSubmissionForEditOutput = await _submissionsAppService.GetSubmissionForEdit(new EntityDto<Guid> { Id = (Guid) id });
		//	}
		//	else{
		//		getSubmissionForEditOutput = new GetSubmissionForEditOutput{
		//			Submission = new CreateOrEditSubmissionDto()
		//		};
		//	}

		//	var viewModel = new CreateOrEditSubmissionModalViewModel()
		//	{
		//		Submission = getSubmissionForEditOutput.Submission,
		//		RecordRecordName = getSubmissionForEditOutput.RecordRecordName,
		//		RecordMatterRecordMatterName = getSubmissionForEditOutput.RecordMatterRecordMatterName,
		//		UserName = getSubmissionForEditOutput.UserName,
		//		AppJobName = getSubmissionForEditOutput.AppJobName
		//	};

		//	return PartialView("_CreateOrEditModal", viewModel);
		//}


		[AbpMvcAuthorize(AppPermissions.Pages_Submissions_Create, AppPermissions.Pages_Submissions_Edit)]
		public PartialViewResult RecordLookupTableModal(Guid? id, string displayName)
		{
			var viewModel = new SubmissionRecordLookupTableViewModel()
			{
				Id = id.ToString(),
				DisplayName = displayName,
				FilterText = ""
			};

			return PartialView("_SubmissionRecordLookupTableModal", viewModel);
		}
		[AbpMvcAuthorize(AppPermissions.Pages_Submissions_Create, AppPermissions.Pages_Submissions_Edit)]
		public PartialViewResult RecordMatterLookupTableModal(Guid? id, string displayName)
		{
			var viewModel = new SubmissionRecordMatterLookupTableViewModel()
			{
				Id = id.ToString(),
				DisplayName = displayName,
				FilterText = ""
			};

			return PartialView("_SubmissionRecordMatterLookupTableModal", viewModel);
		}

		[AbpMvcAuthorize(AppPermissions.Pages_Submissions)]
		public async Task<PartialViewResult> ViewSubmissionModal(Guid Id)
		{
			GetSubmissionForViewOutput Output = await _submissionsAppService.GetSubmissionForView(new EntityDto<Guid> { Id = Id });
			SubmissionViewViewModel viewModel = new SubmissionViewViewModel()
			{
				Id = (Guid)Output.Submission.Id,
				CreationTime = Output.Submission.CreationTime,
				LastModified = Output.Submission.LastModificationTime ?? null,
				UserName = Output.UserName,
				UserEmail = Output.UserEmail,
				Type = Output.Submission.Type,
				SubmissionStatus = Output.Submission.SubmissionStatus,
				RecordName = Output.RecordName,
				RecordMatterName = Output.RecordMatterName,
				RecordMatterItems = Output.RecordMatterItems,
				RequiresPayment = Output.Submission.RequiresPayment,
				PaymentCurrency = Output.Submission.PaymentCurrency,
				PaymentStatus = Output.Submission.PaymentStatus,
				PaymentAmount = Output.Submission.PaymentAmount,
				VoucherAmount = Output.Submission.VoucherAmount,
				AmountPaid = Output.Submission.AmountPaid,
				ChargeId = Output.Submission.ChargeId
			};
			return PartialView("_ViewSubmissionModal", viewModel);
		}

		[AbpMvcAuthorize(AppPermissions.Pages_Submissions_Create, AppPermissions.Pages_Submissions_Edit)]
		public PartialViewResult AppJobLookupTableModal(Guid? id, string displayName)
		{
			var viewModel = new SubmissionAppJobLookupTableViewModel()
			{
				Id = id.ToString(),
				DisplayName = displayName,
				FilterText = ""
			};

			return PartialView("_SubmissionAppJobLookupTableModal", viewModel);
		}

	}
}