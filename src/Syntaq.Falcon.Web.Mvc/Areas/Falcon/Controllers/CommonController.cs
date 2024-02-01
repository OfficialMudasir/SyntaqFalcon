using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Authorization.Permissions;
using Syntaq.Falcon.Authorization.Permissions.Dto;
using Syntaq.Falcon.Users;
using Syntaq.Falcon.Users.Dtos;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Common.Modals;
using Syntaq.Falcon.Web.Areas.Falcon.Models.UserAcceptances;
using Syntaq.Falcon.Web.Controllers;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize]
    public class CommonController : FalconControllerBase
    {
        private readonly IPermissionAppService _permissionAppService;
        private readonly IUserAcceptanceTypesAppService _userAcceptanceTypesAppService;

        public CommonController(IUserAcceptanceTypesAppService userAcceptanceTypesAppService, IPermissionAppService permissionAppService)
        {
            _permissionAppService = permissionAppService;
            _userAcceptanceTypesAppService = userAcceptanceTypesAppService;
        }

        public PartialViewResult LookupModal(LookupModalViewModel model)
        {
            return PartialView("Modals/_LookupModal", model);
        }

        public PartialViewResult EntityTypeHistoryModal(EntityHistoryModalViewModel input)
        {
            return PartialView("Modals/_EntityTypeHistoryModal", ObjectMapper.Map<EntityHistoryModalViewModel>(input));
        }

        public PartialViewResult PermissionTreeModal(List<string> grantedPermissionNames = null)
        {
            var permissions = _permissionAppService.GetAllPermissions().Items.ToList();

            var model = new PermissionTreeModalViewModel
            {
                Permissions = ObjectMapper.Map<List<FlatPermissionDto>>(permissions).OrderBy(p => p.DisplayName).ToList(),
                GrantedPermissionNames = grantedPermissionNames
            };

            return PartialView("Modals/_PermissionTreeModal", model);
        }

        public PartialViewResult InactivityControllerNotifyModal()
        {
            return PartialView("Modals/_InactivityControllerNotifyModal");
        }

        public async Task<PartialViewResult> UserAcceptanceGlobalModal(Guid? contributorId, long? userId)
        {
            List<UserAcceptanceTypeDto> activeUserAcceptanceTypesList = await _userAcceptanceTypesAppService.GetAllActiveUserAcceptanceTypesForView();

            var viewModel = new UserAcceptancesAcceptViewModel()
            {
                ActiveUserAcceptanceTypesList = activeUserAcceptanceTypesList, 
                UserId = userId, 
                RecordMatterContributorId = contributorId
            };

            return PartialView("Modals/_UserAcceptanceModal", viewModel);
        }
    }
}