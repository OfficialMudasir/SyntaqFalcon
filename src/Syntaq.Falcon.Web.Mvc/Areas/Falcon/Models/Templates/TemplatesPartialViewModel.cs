using Syntaq.Falcon.Folders.Dtos;
using System.Collections.Generic;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Templates
{
    public class TemplatesPartialViewModel
    {
        public string FilterText { get; set; }

        public List<FolderDto> BreadcrumbList { get; set; }

        public int TotalBreadcrumbCount { get; set; }

        public string CurrentFolder { get; set; }
    }
}
