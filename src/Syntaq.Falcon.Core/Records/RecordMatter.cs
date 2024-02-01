using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Organizations;
using Newtonsoft.Json;
using Syntaq.Falcon.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Syntaq.Falcon.Records.RecordMatterConsts;

namespace Syntaq.Falcon.Records
{
	[Table("SfaRecordMatters")]
	[Abp.Auditing.Audited]
	public class RecordMatter : FullAuditedEntity<Guid> , IMayHaveTenant
	{
		public int? TenantId { get; set; }

		public virtual string Data { get; set; }

		[StringLength(RecordMatterConsts.MaxAccessTokenLength, MinimumLength = RecordMatterConsts.MinAccessTokenLength)]
		public virtual string AccessToken { get; set; }

		public virtual string RecordMatterName { get; set; }

		public virtual string Key { get; set; }

		[StringLength(RecordMatterConsts.MaxCommentsLength, MinimumLength = RecordMatterConsts.MinCommentsLength)]
		public virtual string Comments { get; set; }
		 
		public virtual Guid RecordId { get; set; }

		[JsonIgnore]
		[ForeignKey("RecordId")]
		public Record Record { get; set; }

		public virtual long? UserId { get; set; }
		[JsonIgnore]
		public User User { get; set; }

		public virtual long? OrganizationUnitId { get; set; }
		[JsonIgnore]
		public OrganizationUnit OrganizationUnit { get; set; }

		public virtual bool HasFiles { get; set; } = false;

		[JsonIgnore]
		public IEnumerable<RecordMatterItem> RecordMatterItems { get; set; } = new List<RecordMatterItem>();

		//[StringLength(RecordMatterConsts.MaxStatusLength, MinimumLength = RecordMatterConsts.MinStatusLength)]
		public RecordMatterStatus? Status { get; set; }



		[StringLength(RecordMatterConsts.MaxRulesSchemaLength, MinimumLength = RecordMatterConsts.MinRulesSchemaLength)]
		public string RulesSchema { get; set; }

		public virtual Guid? FormId { get; set; }

		public int Order { get; set; }
		public bool RequireReview { get; set; }
		public bool RequireApproval { get; set; }

		/// <summary>
		/// filters recordmatters used in Projects as steps
		/// </summary>
		public virtual string Filter { get; set; }
	}
}