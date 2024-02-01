

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System.Text.Json.Serialization;

namespace Syntaq.Falcon.Folders
{
	[Table("SfaFolders")]
	[Abp.Auditing.Audited]
	public class Folder : FullAuditedEntity<Guid> , IMayHaveTenant
	{
		public int? TenantId { get; set; }

		[Required]
		[StringLength(FolderConsts.MaxNameLength, MinimumLength = FolderConsts.MinNameLength)]
		public virtual string Name { get; set; }
		
		[StringLength(FolderConsts.MaxDescriptionLength, MinimumLength = FolderConsts.MinDescriptionLength)]
		public virtual string Description { get; set; }
		
		public virtual Guid? ParentId { get; set; }
		
        public virtual string Type { get; set; }

        [ForeignKey("ParentId")]
		[JsonIgnore]
		public Folder ParentFolder { get; set; }

        [NotMapped]
        public string ACLRole { get; set; }

        [NotMapped]
        public long? OrganizationUnitId { get; set; }
    }
}