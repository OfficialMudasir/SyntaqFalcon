using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Documents;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Utility;
using Syntaq.Falcon.Web.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
 
    [Area("Falcon")]
	public class DocumentsController : FalconControllerBase
	{
		private readonly IDocumentsAppService _documentsAppService;
        private readonly IRepository<RecordMatterItem, Guid> _recordMatterItemRepository;
        private readonly IRecordMatterItemsAppService _recordMatterItemsAppService;

        public DocumentsController(IDocumentsAppService documentsAppService, IRepository<RecordMatterItem, Guid> recordMatterItemRepository, IRecordMatterItemsAppService recordMatterItemsAppService)
		{
			_documentsAppService = documentsAppService;
            _recordMatterItemRepository = recordMatterItemRepository;
            _recordMatterItemsAppService = recordMatterItemsAppService;
        }

        [Authorize(Policy = "ViewById")]
        public FileResult GetDocumentForDownload(Guid Id, int version, string format, string AccessToken)
        {
            AccessToken = AccessToken == "null" ? "" : AccessToken;

            Task<GetRecordMatterItemForDownload> getTemplateForEditOutput = _recordMatterItemsAppService.GetDocumentForDownload(new EntityDto<Guid> { Id = Id }, version, format, AccessToken);
            if (getTemplateForEditOutput.Result.RecordMatterItem.Type != RecordMatterItemForDownloadType.Disallow)
            {
                byte[] fileBytes = getTemplateForEditOutput.Result.RecordMatterItem.Document;
                string fileName = getTemplateForEditOutput.Result.RecordMatterItem.DocumentName;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else
            {
                throw new UserFriendlyException("You are not authorised to download this file type.");
            }
        }

        [Authorize(Policy = "ViewById")]
        public FileResult GetPDFForDownload(Guid Id, int version, string AccessToken)
        {
            AccessToken = AccessToken == "null" ? "" : AccessToken;

            Task<GetRecordMatterItemForDownload> getTemplateForEditOutput = _recordMatterItemsAppService.GetDocumentForDownload(new EntityDto<Guid> { Id = Id }, version, "pdf", AccessToken);
            if (getTemplateForEditOutput.Result.RecordMatterItem.Type != RecordMatterItemForDownloadType.Disallow)
            {
                byte[] fileBytes = getTemplateForEditOutput.Result.RecordMatterItem.Document;
                string fileName = getTemplateForEditOutput.Result.RecordMatterItem.DocumentName;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);
            }
            else
            {
                throw new UserFriendlyException("You are not authorised to download this file type.");
            }
        }

        //[Authorize(Policy = "ViewById")]
        //public async Task<GetRecordMatterItemForDownload> GetDocumentForDownload(EntityDto<Guid> input, int version, string format, string AccessToken)
        //{
        //    var recordmatteritem = await _recordMatterItemRepository.GetAll().Where(i => i.Id == input.Id).FirstOrDefaultAsync();
        //    Byte[] bydoc = recordmatteritem.Document;
        //    var AllowedFormats = recordmatteritem.AllowedFormats;
        //    RecordMatterItemForDownloadType type = RecordMatterItemForDownloadType.PDF;

        //    if (bydoc != null)
        //    {
        //        if (AllowedFormats.Contains("W") && format == "docx")
        //        {
        //            type = RecordMatterItemForDownloadType.Word;
        //        }
        //        else if (AllowedFormats.Contains("P") && format == "pdf")
        //        {
        //            bydoc = AsposeUtility.BytesToPdf(bydoc);
        //            type = RecordMatterItemForDownloadType.PDF;
        //        }
        //        else if (AllowedFormats.Contains("H") && format == "html")
        //        {
        //            bydoc = AsposeUtility.BytesToHTML(bydoc);
        //            type = RecordMatterItemForDownloadType.HTML;
        //        }
        //        else
        //        {
        //            type = RecordMatterItemForDownloadType.Disallow;
        //        }
        //    }

        //    var filename = Path.GetFileNameWithoutExtension(recordmatteritem.DocumentName);
        //    var output = new GetRecordMatterItemForDownload
        //    {
        //        RecordMatterItem = new RecordMatterItemForDownloadDto
        //        {
        //            Document = type != RecordMatterItemForDownloadType.Disallow ? bydoc : null,
        //            DocumentName = type != RecordMatterItemForDownloadType.Disallow ? filename + "." + format : null,
        //            Type = type
        //        }
        //    };
        //    return output;
        //}
    }
}