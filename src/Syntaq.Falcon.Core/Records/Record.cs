using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Organizations;
using Newtonsoft.Json;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Folders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Syntaq.Falcon.Records
{
	[Table("SfaRecords")]
	[Abp.Auditing.Audited]
	public class Record : FullAuditedEntity<Guid>, IMayHaveTenant
	{
		public int? TenantId { get; set; }

		[Required]
		[StringLength(RecordConsts.MaxRecordNameLength, MinimumLength = RecordConsts.MinRecordNameLength)]
		public virtual string RecordName { get; set; } = "Record name not Set";

		public virtual string Data { get; set; }

		public virtual Guid? FolderId { get; set; }

		[JsonIgnore]
		public Folder Folder { get; set; }

		public string Comments { get; set; }

		public virtual long? UserId { get; set; }
		[JsonIgnore]
		public User User { get; set; }

		public virtual long? OrganizationUnitId { get; set; }
		[JsonIgnore]
		public OrganizationUnit OrganizationUnit { get; set; }

		public IEnumerable<RecordMatter> RecordMatters { get; set; } = new List<RecordMatter>();

		public string AccessToken { get; set; }

        public DateTime? Locked { get; set; }

		public bool IsArchived { get; set; }

    }
}