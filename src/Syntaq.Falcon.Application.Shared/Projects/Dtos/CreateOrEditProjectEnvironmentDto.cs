using Syntaq.Falcon.Projects;

using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class CreateOrEditProjectEnvironmentDto : EntityDto<Guid?>
    {

        [Required]
        [StringLength(ProjectEnvironmentConsts.MaxNameLength, MinimumLength = ProjectEnvironmentConsts.MinNameLength)]
        public string Name { get; set; }

        [StringLength(ProjectEnvironmentConsts.MaxDescriptionLength, MinimumLength = ProjectEnvironmentConsts.MinDescriptionLength)]
        public string Description { get; set; }

        public ProjectEnvironmentConsts.ProjectEnvironmentType EnvironmentType { get; set; }

    }
}