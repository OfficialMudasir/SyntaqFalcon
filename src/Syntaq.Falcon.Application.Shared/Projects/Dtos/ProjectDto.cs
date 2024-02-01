using Syntaq.Falcon.Projects;

using System;
using Abp.Application.Services.Dto;
using static Syntaq.Falcon.Projects.ProjectConsts;
using Syntaq.Falcon.Records.Dtos;
using System.Collections.Generic;

namespace Syntaq.Falcon.Projects.Dtos
{
	public class ProjectDto : EntityDto<Guid>
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public DateTime CreationTime { get; set; }
		public long CreatorUserId { get; set; }
		public String CreatorUserName { get; set; }

		public DateTime LastModificationTime { get; set; }
		public long LastModifiedUserId { get; set; }
		public String LastModifiedUserName { get; set; }

		public ProjectStatus Status { get; set; }

		public ProjectType Type { get; set; }

		public Guid? RecordId { get; set; }
        public Guid? ReleaseId { get; set; }
        public RecordDto Record { get; set; }

		public List<RecordMatterContributorDto> Contributors { get; set; }

        public String ReleaseNotes { get; set; } = String.Empty;
        public virtual int VersionMajor { get; set; }
        public virtual int VersionMinor { get; set; }
        public virtual int VersionRevision { get; set; }
        public DateTime ReleaseDate { get; set; }
        public virtual Guid? ProjectEnvironmentId { get; set; }
        public virtual String ProjectEnvironmentName { get; set; } = String.Empty;
		public virtual ProjectEnvironmentConsts.ProjectEnvironmentType ProjectEnvironmentType { get; set; }
        public virtual String ProjectEnvironmentTypeDescription { get; set; }
    }
}