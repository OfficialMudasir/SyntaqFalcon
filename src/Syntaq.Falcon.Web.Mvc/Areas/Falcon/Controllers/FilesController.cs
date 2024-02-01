using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Files;
using Syntaq.Falcon.Files.Dtos;
using Syntaq.Falcon.Web.Areas.Falcon.Models.Files;
using Syntaq.Falcon.Web.Controllers;
using System.IO;

namespace Syntaq.Falcon.Web.Areas.Falcon.Controllers
{
	[Area("Falcon")]
	//[AbpMvcAuthorize]
	public class FilesController : FalconControllerBase
	{
		private readonly IFilesAppService _filesAppService;

		public FilesController(IFilesAppService filesAppService)
		{
			_filesAppService = filesAppService;
		}

		public IActionResult Index()
		{
			return View();
		}

		//[Authorize(Policy = "EditByRecordMatterId")]
		// Auhorisation is checked the API
		public PartialViewResult ViewFilesModal(string RecordId, string RecordMatterId, string recordMatterItemGroupId, string RecordMatterName)
		{
			var viewModel = new FilesViewModel()
			{
				RecordId = RecordId,
				RecordMatterId = RecordMatterId,
				RecordMatterName = RecordMatterName,
                RecordMatterItemGroupId = recordMatterItemGroupId
            };
			return PartialView("_ViewFilesModal", viewModel);
		}

        //[Authorize(Policy = "EditByRecordMatterId")]
        // Auhorisation is checked the API
        public ActionResult DownloadFile(FilesDto filesDto)
		{
			var returnStream = _filesAppService.DownloadFileById(filesDto);

            if (returnStream == null)
                return new Microsoft.AspNetCore.Mvc.NotFoundResult();

            returnStream.FileStream.Position = 0;

            byte[] Content = new BinaryReader(returnStream.FileStream).ReadBytes(System.Convert.ToInt32( returnStream.FileStream.Length ));
            return File(Content, System.Net.Mime.MediaTypeNames.Application.Octet, filesDto.FileName);

		}

        public static byte[] streamToByteArray(Stream stream)
        {
            byte[] byteArray = new byte[16 * 1024];
            using (MemoryStream mSteram = new MemoryStream())
            {
                int bit;
                while ((bit = stream.Read(byteArray, 0, byteArray.Length)) > 0)
                {
                    mSteram.Write(byteArray, 0, bit);
                }
                return mSteram.ToArray();
            }
        }

        //[Authorize(Policy = "EditByRecordMatterId")]
        // Auhorisation is checked the API
        public FileResult DownloadAllFiles(FilesDto filesDto)
		{
			MemoryStream returnedFiles = _filesAppService.DownloadAllFilesByRecordMatterId(filesDto);

            byte[] returnedFileByte = returnedFiles.ToArray();
			return File(returnedFileByte, "application/zip", "DownloadAll.zip");
		}
	}
}