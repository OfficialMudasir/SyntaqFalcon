using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.ProjectTemplates.Dtos
{
    public class GetProjectTemplatesForViewDto : EntityDto<Guid?>
    {
        public string Name { get; set; }
        public string TenancyName { get; set; }
        public string Description { get; set; }
        public DateTime CreationTime { get; set; }
        public bool Enabled { get; set; }
        public bool Accepted { get; set; }
        public bool Shared { get; set; }

        public ProjectTemplateType Type { get; set; }

        public Guid? ProjectReleaseId { get; set; }

    }

    public enum ProjectTemplateType
    {
        Template,
        Deployment
    }
}
