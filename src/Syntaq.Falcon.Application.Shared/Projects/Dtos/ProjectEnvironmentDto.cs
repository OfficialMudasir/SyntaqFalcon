using Syntaq.Falcon.Projects;

using System;
using Abp.Application.Services.Dto;
using static Syntaq.Falcon.Projects.ProjectEnvironmentConsts;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class ProjectEnvironmentDto : EntityDto<Guid>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ProjectEnvironmentType EnvironmentType { get; set; }

    }
}