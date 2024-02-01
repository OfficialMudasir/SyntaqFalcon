using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class GetProjectsRecentTemplatesForDashboardDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectStepName { get; set; }
        public string DocumentName { get; set; }
        public DateTime LastModificationTime { get; set; }
        public Guid? FormId { get; set; }
        public Guid RecordMatterId { get; set; }
        public Guid RecordMatterItemId { get; set; }

        public string AccessToken { get; set; }
        public char Type { get; set; }
        public DateTime CreationTime { get; set; }
        public string AllowedFormats { get; set; }
        public string Status { get; set; }
       
    }
}
