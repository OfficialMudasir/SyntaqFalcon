using Syntaq.Falcon.Projects;

using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class CreateOrEditProjectReleaseDto : EntityDto<Guid?>
    {

        public Guid? ProjectReleaseId { get; set; }

        [Required]
        [StringLength(ProjectReleaseConsts.MaxNameLength, MinimumLength = ProjectReleaseConsts.MinNameLength)]
        public string Name { get; set; }

        [StringLength(ProjectReleaseConsts.MaxNotesLength, MinimumLength = ProjectReleaseConsts.MinNotesLength)]
        public string Notes { get; set; }

        public Guid ProjectTemplateId { get; set; }
        public Guid ProjectId { get; set; }

        public bool Required { get; set; }

        public int VersionMajor { get; set; }

        public int VersionMinor { get; set; }

        public int VersionRevision { get; set; }

        public ProjectReleaseEnums.ProjectReleaseType ReleaseType { get; set; }

        public Guid? ProjectEnvironmentId { get; set; }

        public List<int> ProjectTenants { get; set; } = new List<int>();

        public Guid ReleaseIdToClone { get; set; }

        public bool DeployToSubscribers { get; set; }
        
    }
}