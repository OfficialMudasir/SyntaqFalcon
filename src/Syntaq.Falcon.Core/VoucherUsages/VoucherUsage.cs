using Syntaq.Falcon.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Syntaq.Falcon.Vouchers;

namespace Syntaq.Falcon.VoucherUsages
{
	[Table("SfaVoucherUsages")]
	public class VoucherUsage : Entity<Guid> , IMayHaveTenant
	{
		public int? TenantId { get; set; }
			

		public virtual DateTime DateRedeemed { get; set; }
		
		[Required]
		[StringLength(VoucherUsageConsts.MaxEntityKeyLength, MinimumLength = VoucherUsageConsts.MinEntityKeyLength)]
		public virtual string EntityKey { get; set; } // NEED THIS??

		//[StringLength(VoucherUsageConsts.MaxEntityTypeLength)]
		//public virtual string EntityName { get; set; }

		[Required]
		[StringLength(VoucherUsageConsts.MaxEntityTypeLength, MinimumLength = VoucherUsageConsts.MinEntityTypeLength)]
		public virtual string EntityType { get; set; }
		

		public virtual long UserId { get; set; }

		public virtual Guid VoucherId { get; set; }


		[ForeignKey("UserId")]
		public User UserFk { get; set; }

		[ForeignKey("VoucherId")]
		public Voucher VoucherFk { get; set; }

	}
}