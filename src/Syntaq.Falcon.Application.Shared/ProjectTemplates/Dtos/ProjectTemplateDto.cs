using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.ProjectTemplates.Dtos
{
    public class ProjectTemplateDto : EntityDto<Guid?>
    {
        public string Name { get; set; }

        //public string TemplateGroup { get; set; }

        public string Description { get; set; }

        public List<ProjectTemplateStepDto> StepsSchema { get; set; }

        //public string TemplateRulesSchema { get; set; }

        public bool IsEnabled { get; set; } = true;
    }
}
