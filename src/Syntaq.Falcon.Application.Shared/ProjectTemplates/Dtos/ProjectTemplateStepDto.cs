using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Syntaq.Falcon.ProjectTemplates.Dtos
{
    public class ProjectTemplateStepDto
    {
        [Required]
        public string StepName { get; set; }
        [Required]
        public Guid? FormId { get; set; }
        public Guid? RecordMatterId { get; set; }
        public string Description { get; set; }
        public string Filter { get; set; }
        public int Order { get; set; } = 0;
        public bool RequireApproval { get; set; }

    }
}
