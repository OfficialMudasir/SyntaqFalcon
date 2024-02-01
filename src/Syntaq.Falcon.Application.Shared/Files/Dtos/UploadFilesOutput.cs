using Abp.Web.Models;

namespace Syntaq.Falcon.Files.Dtos
{
    public class UploadFilesOutput : ErrorInfo
    {
        public string FileName { get; set; }

        public string FileType { get; set; }

        public string FileToken { get; set; }

        public UploadFilesOutput()
        {

        }

        public UploadFilesOutput(ErrorInfo error)
        {
            Code = error.Code;
            Details = error.Details;
            Message = error.Message;
            ValidationErrors = error.ValidationErrors;
        }
    }
}
