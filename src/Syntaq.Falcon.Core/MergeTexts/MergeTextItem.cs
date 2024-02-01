using Abp.Domain.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Syntaq.Falcon.MergeTexts
{
	[Table("SfaMergeTextItems")]
	[Abp.Auditing.Audited]
	public class MergeTextItem : Entity<long> , IMayHaveTenant
	{
		public int? TenantId { get; set; }

		[Required]
		[StringLength(MergeTextItemConsts.MaxNameLength, MinimumLength = MergeTextItemConsts.MinNameLength)]
		public virtual string Name { get; set; }

		public virtual long MergeTextId { get; set; }

		[ForeignKey("MergeTextId")]
		public MergeText MergeTextFk { get; set; }

		public List<MergeTextItemValue> MergeTextItemValues { get; set; } = new List<MergeTextItemValue>();
	}
}