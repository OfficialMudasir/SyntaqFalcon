using Syntaq.Falcon.Projects;
using Syntaq.Falcon.Records;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;
using static Syntaq.Falcon.Projects.ProjectConsts;
using Castle.Core;
using System.Collections;
using System.Collections.Generic;
using Syntaq.Falcon.Authorization.Users;

namespace Syntaq.Falcon.Projects
{
	[Table("sfaProjects")]
    [Audited]
    public class Project : FullAuditedEntity<Guid> , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[StringLength(ProjectConsts.MaxNameLength, MinimumLength = ProjectConsts.MinNameLength)]
		public virtual string Name { get; set; }

		public virtual string Filter { get; set; }

		[StringLength(ProjectConsts.MaxDescriptionLength, MinimumLength = ProjectConsts.MinDescriptionLength)]
		public virtual string Description { get; set; }
		
		public virtual ProjectStatus Status { get; set; }
		
		public virtual ProjectType Type { get; set; }
		
		public virtual Guid? RecordId { get; set; }
		
        [ForeignKey("RecordId")]
		public Record RecordFk { get; set; }
		
		public bool Enabled { get; set; }
		public bool Archived { get; set; }
		public virtual Guid? ProjectTemplateId { get; set; }

		[ForeignKey("CreatorUserId")]
		public User UserFk { get; set; }

        public virtual Guid? ProjectId { get; set; }
		public virtual int Version { get; set; }

        public virtual Guid? ReleaseId { get; set; }
        [ForeignKey("ReleaseId")]
        public ProjectRelease ReleaseFk { get; set; }

        public virtual Guid? ProjectEnvironmentId { get; set; }

    }
}