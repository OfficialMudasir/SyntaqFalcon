using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.ProjectTemplates.Dtos
{
    public class GetProjectTemplateForView
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<ProjectTemplateStepDto> StepsSchema { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime LastModificationTime { get; set; }

        public bool Enabled { get; set; }

        public int Version { get; set; }

    }
}
