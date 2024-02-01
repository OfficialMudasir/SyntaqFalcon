using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.UserAcceptances;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Users;
using Syntaq.Falcon.Users.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using Syntaq.Falcon.Sessions;
using Abp.Runtime.Session;
using Abp.Domain.Repositories;
using Abp.Collections.Extensions;
using System.Linq;
using Syntaq.Falcon.Configuration;
using System.IO;
using System.Text;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Documents;
using Syntaq.Falcon.Records.Dtos;
using Abp.UI;
using Syntaq.Falcon.Utility;
using Abp.Domain.Uow;
using Syntaq.Falcon.Documents.Dtos;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    public class UserAcceptancesController : FalconControllerBase
    {
        private readonly IUserAcceptancesAppService _userAcceptancesAppService;
        private readonly ITemplatesAppService _templatesAppService;

        public UserAcceptancesController(ITemplatesAppService templatesAppService, IRepository<RecordMatterItem, Guid> recordMatterItemRepository, IUserAcceptancesAppService userAcceptancesAppService, IRepository<UserAcceptance, Guid> userAcceptanceRepository, IRepository<UserAcceptanceType, Guid> userAcceptanceTypeRepository)
        {
            _userAcceptancesAppService = userAcceptancesAppService;
            _templatesAppService = templatesAppService;
        }

        [AbpMvcAuthorize(AppPermissions.Pages_UserAcceptances)]
        public ActionResult Index()
        {
            var model = new UserAcceptancesViewModel
            {
                FilterText = ""
            };

            return View(model);
        }
 
        [AbpMvcAuthorize(AppPermissions.Pages_UserAcceptances_Create, AppPermissions.Pages_UserAcceptances_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(Guid? id)
        {
            GetUserAcceptanceForEditOutput getUserAcceptanceForEditOutput;

            if (id.HasValue)
            {
                getUserAcceptanceForEditOutput = await _userAcceptancesAppService.GetUserAcceptanceForEdit(new EntityDto<Guid> { Id = (Guid)id });
            }
            else
            {
                getUserAcceptanceForEditOutput = new GetUserAcceptanceForEditOutput
                {
                    UserAcceptance = new CreateOrEditUserAcceptanceDto()
                };
            }

            var viewModel = new CreateOrEditUserAcceptanceModalViewModel()
            {
                UserAcceptance = getUserAcceptanceForEditOutput.UserAcceptance,
                UserAcceptanceTypeName = getUserAcceptanceForEditOutput.UserAcceptanceTypeName,
                UserName = getUserAcceptanceForEditOutput.UserName,
                RecordMatterContributorName = getUserAcceptanceForEditOutput.RecordMatterContributorName
            };

            return PartialView("_CreateOrEditModal", viewModel);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_UserAcceptances_Create, AppPermissions.Pages_UserAcceptances_Edit)]
        public PartialViewResult UserAcceptanceTypeLookupTableModal(Guid? id, string displayName)
        {
            var viewModel = new UserAcceptanceTypeLookupTableViewModel()
            {
                Id = id.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_UserAcceptanceTypeLookupTableModal", viewModel);
        }
        [AbpMvcAuthorize(AppPermissions.Pages_UserAcceptances_Create, AppPermissions.Pages_UserAcceptances_Edit)]
        public PartialViewResult UserLookupTableModal(long? id, string displayName)
        {
            var viewModel = new UserLookupTableViewModel()
            {
                Id = id,
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_UserLookupTableModal", viewModel);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_UserAcceptances_Create, AppPermissions.Pages_UserAcceptances_Edit)]
        public PartialViewResult RecordMatterContributorLookupTableModal(Guid? id, string displayName)
        {
            var viewModel = new RecordMatterContributorLookupTableViewModel()
            {
                Id = id.ToString(),
                DisplayName = displayName,
                FilterText = ""
            };

            return PartialView("_RecordMatterContributorLookupTableModal", viewModel);
        }

        public async Task<PartialViewResult> UserAcceptanceDocumentModal(Guid? id)
        {
            GetTemplateForEditOutput getTemplateForEditOutput = await _templatesAppService.GetTemplateForUserAcceptanceViewers(new EntityDto<Guid> { Id = (Guid)id });
            Byte[] bydoc = getTemplateForEditOutput.Template.Document;
            bydoc = AsposeUtility.BytesToHTML(bydoc);
            string formatedTempalte = Convert.ToBase64String(bydoc);

            string finalResult = string.Format("data:text/html;base64,{0}", formatedTempalte);

            var viewModel = new UserAcceptancesDocumentViewModel()
            {
                Content = finalResult
            };
            return PartialView("_UserAcceptanceDocumentModal", viewModel);
        }

        [AllowAnonymous]
        public ActionResult UserAcceptance(Guid? RecordMatterContributorId)
        { 

            var userid = AbpSession.UserId;

            var result = _userAcceptancesAppService.GetRequiredAcceptancesForUser(
                new GetRequiredUserAcceptancesInput(){RecordMatterContributorId = RecordMatterContributorId}
            ).Result;

            if (result.Items.Count == 0)
            {
                var query = System.Web.HttpUtility.ParseQueryString(Convert.ToString(Request.QueryString));
                var returnUrl = query.Get("ReturnUrl");
                return Redirect(Convert.ToString(returnUrl));
            }

            var model = ObjectMapper.Map<List<UserAcceptanceViewModel>>(result.Items);
            return View(model);
        }
 
    }
}