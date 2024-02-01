using Abp.Domain.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Syntaq.Falcon.MergeTexts
{
	[Table("SfaMergeTexts")]
	[Abp.Auditing.Audited]
	public class MergeText : Entity<long> , IMayHaveTenant
	{
		public int? TenantId { get; set; }

		[Required]
		[StringLength(MergeTextConsts.MaxEntityTypeLength, MinimumLength = MergeTextConsts.MinEntityTypeLength)]
		public virtual string EntityType { get; set; }
		
		[Required]
		[StringLength(MergeTextConsts.MaxEntityKeyLength, MinimumLength = MergeTextConsts.MinEntityKeyLength)]
		public virtual string EntityKey { get; set; }

		public List<MergeTextItem> MergeTextItems { get; set; } = new List<MergeTextItem>();
	}
}