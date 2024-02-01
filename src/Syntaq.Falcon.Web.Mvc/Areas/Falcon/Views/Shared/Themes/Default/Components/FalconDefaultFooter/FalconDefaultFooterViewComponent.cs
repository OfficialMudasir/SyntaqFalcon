using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Users;
using Syntaq.Falcon.Users.Dtos;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Layout;
using Syntaq.Falcon.Web.Session;
using Syntaq.Falcon.Web.Views;

namespace Syntaq.Falcon.Web.Areas.Falcon.Views.Shared.Themes.Default.Components.FalconDefaultFooter
{
    public class FalconDefaultFooterViewComponent : FalconViewComponent
    {
        private readonly IPerRequestSessionCache _sessionCache;
        private readonly IUserAcceptanceTypesAppService _userAcceptanceTypesAppService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;


        public FalconDefaultFooterViewComponent(IUnitOfWorkManager unitOfWorkManager, IUserAcceptanceTypesAppService userAcceptanceTypesAppService, IPerRequestSessionCache sessionCache)
        {
            _userAcceptanceTypesAppService = userAcceptanceTypesAppService;
            _sessionCache = sessionCache;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<UserAcceptanceTypeDto> activeUserAcceptanceTypesList = await _userAcceptanceTypesAppService.GetAllActiveUserAcceptanceTypesForView();

            var footerModel = new FooterViewModel
            {
                LoginInformations = await _sessionCache.GetCurrentLoginInformationsAsync(),
                ActiveUserAcceptanceTypesList = activeUserAcceptanceTypesList
            };

            return View(footerModel);
        }
    }
}
