using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Asic;
using Syntaq.Falcon.Web.Controllers;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.ASIC;
using Syntaq.Falcon.ASIC.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Linq;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
    [Area("Falcon")]
    [AbpMvcAuthorize(AppPermissions.Pages_Asic)]
    public class AsicController : FalconControllerBase
    {
        private readonly IAsicAppService _asicAppService;

        public AsicController(IAsicAppService asicAppService)
        {
            _asicAppService = asicAppService;

        }

        public ActionResult Index()
        {
            var model = new AsicViewModel
            {
                FilterText = ""
            };

            return View(model);
        }

        // No Public Method
        //[AbpMvcAuthorize(AppPermissions.Pages_Asic_Create, AppPermissions.Pages_Asic_Edit)]
        //public async Task<PartialViewResult> CreateOrEditModal(Guid? id)
        //{
        //    GetAsicForEditOutput getAsicForEditOutput;

        //    if (id.HasValue)
        //    {
        //        getAsicForEditOutput = await _asicAppService.GetAsicForEdit(new EntityDto<Guid> { Id = (Guid)id });
        //    }
        //    else
        //    {
        //        getAsicForEditOutput = new GetAsicForEditOutput
        //        {
        //            Asic = new CreateOrEditAsicDto()
        //        };
        //    }

        //    var viewModel = new CreateOrEditAsicModalViewModel()
        //    {
        //        Asic = getAsicForEditOutput.Asic,

        //    };

        //    return PartialView("_CreateOrEditModal", viewModel);
        //}

        // No Public Method
        //public async Task<PartialViewResult> ViewAsicModal(Guid id)
        //{
        //    var getAsicForViewDto = await _asicAppService.GetAsicForView(id);

        //    var model = new AsicViewModel()
        //    {
        //        Asic = getAsicForViewDto.Asic
        //    };

        //    return PartialView("_ViewAsicModal", model);
        //}

        public FileResult Get201Certificate(string requestId)
        {
            Task<byte[]> fileBytesresult = _asicAppService.Get201Certificate(Int32.Parse(requestId));
            byte[] fileBytes = fileBytesresult.Result;
            string fileName = requestId + "-certificate.pdf";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);
        }

        //https://localhost:44302/Falcon/ASIC/GetAllAsicOfficial201Documents?requestId=%25563
        public FileResult GetAllAsicOfficial201Documents(int requestId)
        {
            Task<byte[]> fileBytesresult = _asicAppService.GetAsic201Documents(requestId);
            byte[] fileBytes = fileBytesresult.Result;
            string fileName = requestId + "-Asic.zip";
            return File(fileBytes.ToArray(), "application/zip", fileName);
        }

        public FileResult DownloadAllDocumentsByRequestId(int requestId)
        {
            MemoryStream fileBytesresult = _asicAppService.DownloadAllDocumentsByRequestId(requestId);
            byte[] fileBytes = fileBytesresult.ToArray();
            string fileName = $"ASIC_201_Docs_${requestId}.zip";
            return File(fileBytes.ToArray(), "application/zip", fileName);
        }

       
    }
}