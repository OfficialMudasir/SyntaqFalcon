using Abp.Application.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Forms.Dtos;
using Syntaq.Falcon.MultiTenancy;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Embedding;
using Syntaq.Falcon.Web.Controllers;
using System;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Web.Mvc.Areas.Falcon.Controllers
{
	[Area("Falcon")]
	public class EmbeddingController : FalconControllerBase
	{
		private readonly IFormsAppService _formsAppService;
		private readonly TenantManager _tenantManager;

		public EmbeddingController(
			IFormsAppService formsAppService,
			TenantManager tenantManager)
		{
			_formsAppService = formsAppService;
			_tenantManager = tenantManager;

		}

		public IActionResult Index()
		{
			return View();
		}

		public async Task<PartialViewResult> CreateModal(Guid? OriginalId, Guid? FormId, string Type)
		{

			var tenancyName = "";
			if (AbpSession.TenantId != null)
			{
				var tenant = _tenantManager.GetById((int)AbpSession.TenantId);
				if (tenant != null)
				{
					tenancyName = tenant.TenancyName;
				}
			}
			
			var viewModel = new GenerateEmbedCodeModalViewModel()
			{
				TenancyName = tenancyName,
				FormId = string.IsNullOrEmpty(OriginalId.ToString()) ? null : OriginalId,
				Type = Type,
				VersionHistory = string.IsNullOrEmpty(OriginalId.ToString()) ? null : await _formsAppService.GetVersionHistory(new EntityDto<Guid>((Guid)OriginalId))
			};
			return PartialView("_CreateModal", viewModel);
		}
	}
}