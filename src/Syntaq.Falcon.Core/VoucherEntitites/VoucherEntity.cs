using Syntaq.Falcon.Vouchers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Syntaq.Falcon.VoucherEntitites
{
    [Table("SfaVoucherEntities")]
    public class VoucherEntity : Entity<Guid> , IMayHaveTenant
    {
		public int? TenantId { get; set; }
			
		public virtual Guid EntityKey { get; set; }

        [Required]
		[StringLength(VoucherEntityConsts.MaxEntityTypeLength, MinimumLength = VoucherEntityConsts.MinEntityTypeLength)]
		public virtual string EntityType { get; set; }

        [StringLength(VoucherEntityConsts.MaxEntityTypeLength)]
        public virtual string EntityName { get; set; }

        public virtual Guid VoucherId { get; set; }
		
        [ForeignKey("VoucherId")]
		public Voucher VoucherFk { get; set; }
		
    }
}