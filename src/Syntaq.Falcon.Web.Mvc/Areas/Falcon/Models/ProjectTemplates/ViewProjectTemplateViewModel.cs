using Newtonsoft.Json.Linq;
using Syntaq.Falcon.ProjectTemplates.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.ProjectTemplates
{
    public class ViewProjectTemplateViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime LastModificationTime { get; set; }

        public bool Enabled { get; set; }

        public List<ProjectTemplateStepDto> StepsSchema { get; set; }
    }
}
