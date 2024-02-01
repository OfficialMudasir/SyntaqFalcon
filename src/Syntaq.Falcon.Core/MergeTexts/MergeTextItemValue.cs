using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Syntaq.Falcon.MergeTexts
{
	[Table("SfaMergeTextItemValues")]
	[Abp.Auditing.Audited]
	public class MergeTextItemValue : Entity<long> , IMayHaveTenant
	{
		public int? TenantId { get; set; }

		[StringLength(MergeTextItemValueConsts.MaxKeyLength, MinimumLength = MergeTextItemValueConsts.MinKeyLength)]
		public virtual string Key { get; set; } = "";

		[StringLength(MergeTextItemValueConsts.MaxValueLength, MinimumLength = MergeTextItemValueConsts.MinValueLength)]
		public virtual string Value { get; set; } = "";

		public virtual long? MergeTextItemId { get; set; }

		[ForeignKey("MergeTextItemId")]
		public MergeTextItem MergeTextItemFk { get; set; }
	}
}