using Abp.IO.Extensions;
using Abp.UI;
using Abp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Syntaq.Falcon.Files;
using Syntaq.Falcon.Files.Dtos;
using Syntaq.Falcon.Storage;
using System.Linq;

namespace Syntaq.Falcon.Web.Controllers
{
    public abstract class FilesControllerBase : FalconControllerBase
    {
        private readonly ITempFileCacheManager _tempFileCacheManager;

        // STQ MODIFIED
        private readonly FilesManager _filesManager;

        protected FilesControllerBase(
            ITempFileCacheManager tempFileCacheManager,
            FilesManager filesManager
        )
        {
            _filesManager = filesManager;
            _tempFileCacheManager = tempFileCacheManager;
        }

        [AllowAnonymous]
        public UploadFilesOutput UploadFile(Dto.FileDto input)
        {
            if (!CheckFileTypeValidation(input.FileType)) {
                return new UploadFilesOutput
                {
                    FileName = input.FileName,
                    FileType = input.FileType
                };
            }
            try
            {
                var UploadedFile = Request.Form.Files.First();

                if (UploadedFile == null)
                {
                    throw new UserFriendlyException(L("ProfilePicture_Change_Error"));
                }

                byte[] fileBytes;
                using (var stream = UploadedFile.OpenReadStream())
                {
                    fileBytes = stream.GetAllBytes();
                }

                Logger.Debug("AVScan: " + input.FileName);

                // STQ MODIFIED AV MALWARE SCANNING
                if (_filesManager.ValidateFile(fileBytes, input.FileName).Result)
                {
                    if (input.IsPrevUpload == true)
                    {
                        _tempFileCacheManager.GetFile(input.FileToken);
                        _tempFileCacheManager.SetFile(input.FileToken, fileBytes);
                    }
                    else
                    {
                        _tempFileCacheManager.SetFile(input.FileToken, fileBytes);
                    }                

                    return new UploadFilesOutput
                    {
                        FileToken = input.FileToken,
                        FileName = input.FileName,
                        FileType = input.FileType
                    };
                }
                else
                {
                    throw new UserFriendlyException("Malware scan failed. ");
                }

            }
            catch (UserFriendlyException ex)
            {
                return new UploadFilesOutput(new ErrorInfo(ex.Message));
            }
        }

        //Prevent File Upload Vulnerabilities. TODO keep adding unacceptable file types.
        private bool CheckFileTypeValidation(string fileType)
        {
            switch (fileType)
            {
                case "application/x-msdownload":
                    return false;
                case "application/x-zip-compressed":
                    return false;
                case null:
                    return false;
            }


            return true;
        }
    }
}
