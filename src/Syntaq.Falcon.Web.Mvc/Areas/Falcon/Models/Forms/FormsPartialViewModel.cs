using Syntaq.Falcon.Folders.Dtos;
using System.Collections.Generic;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Forms
{
    public class FormsPartialViewModel
    {
        public string FilterText { get; set; }

        public List<FolderDto> BreadcrumbList { get; set; }

        public int TotalBreadcrumbCount { get; set; }

        public string CurrentFolder { get; set; }
    }
}
