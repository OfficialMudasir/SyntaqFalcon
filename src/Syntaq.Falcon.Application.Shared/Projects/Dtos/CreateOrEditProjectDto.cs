using Syntaq.Falcon.Projects;

using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using static Syntaq.Falcon.Projects.ProjectConsts;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class CreateOrEditProjectDto : EntityDto<Guid?>
    {

		[StringLength(ProjectConsts.MaxNameLength, MinimumLength = ProjectConsts.MinNameLength)]
		public string Name { get; set; }
		
		
		[StringLength(ProjectConsts.MaxDescriptionLength, MinimumLength = ProjectConsts.MinDescriptionLength)]
		public string Description { get; set; }
		
		
		public ProjectStatus Status { get; set; }
		
		
		public ProjectType Type { get; set; }
		
		
		 public Guid? RecordId { get; set; }

		public bool Enabled { get; set; } = true;

        public Guid? ProjectId { get; set; }
        public int Version { get; set; }
        public string VersionDescription { get; set; }

    }
}