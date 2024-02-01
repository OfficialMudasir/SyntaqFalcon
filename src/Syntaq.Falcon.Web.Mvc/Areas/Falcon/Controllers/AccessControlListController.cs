using Abp.Application.Services.Dto;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.Web.Areas.Falcon.Models.AccessControlList;
using Syntaq.Falcon.Web.Controllers;
using System;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
	[Area("Falcon")]
	public class AccessControlListController : FalconControllerBase
	{
		private readonly IACLsAppService _ACLsAppService;

		public AccessControlListController(IACLsAppService ACLsAppService)
		{
			_ACLsAppService = ACLsAppService;
		}

		public PartialViewResult ManageACLModal(Guid EntityId, string EntityName, string EntityType, bool simplemode = false)
		{
			ACLCheckDto aCLCheckDto = new ACLCheckDto()
			{
				Action = "Share",
				EntityId = EntityId,
				UserId = AbpSession.UserId,
				TenantId = AbpSession.TenantId
			};


            try
            {
                ListResultDto<GetACLForEditOutput> EntityACL = _ACLsAppService.GetACLForEdit(aCLCheckDto);

                GetACLForView getACLForEditOutput = new GetACLForView()
                {
                    EntityId = EntityId,
                    EntityName = EntityName,
                    EntityType = EntityType,
                    EntityACL = EntityACL
                };

			var viewModel = new ManageACLModalViewModel(getACLForEditOutput);
			viewModel.SimpleView = simplemode;

                return PartialView("_ManageACLModal", viewModel);
            }
            catch (UserFriendlyException ex)
            {
                throw ex;
            }


		}

		public PartialViewResult ManageTenantACLModal(Guid EntityId, string EntityName, string EntityType)
		{
			ACLCheckDto aCLCheckDto = new ACLCheckDto()
			{
				Action = "Share",
				EntityId = EntityId,
				UserId = AbpSession.UserId,
				Type = ACLCheckType.Tenant
			};

			try
			{
                ListResultDto<GetACLForEditOutput> EntityACL = _ACLsAppService.GetACLForEdit(aCLCheckDto);

                GetACLForView getACLForEditOutput = new GetACLForView()
                {
                    EntityId = EntityId,
                    EntityName = EntityName,
                    EntityType = EntityType,
                    EntityACL = EntityACL
                };

                var viewModel = new ManageACLModalViewModel(getACLForEditOutput);

                return PartialView("_ManageTenantACLModal", viewModel);
            }
			catch (Exception ex)
			{
                throw ex;
            }


		}
	}
}
