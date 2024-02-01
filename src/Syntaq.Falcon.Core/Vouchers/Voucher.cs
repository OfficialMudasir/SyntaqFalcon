using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System.Collections.Generic;
using Syntaq.Falcon.VoucherEntitites.Dtos;

namespace Syntaq.Falcon.Vouchers
{
	[Table("SfaVouchers")]
    public class Voucher : AuditedEntity<Guid> , IMayHaveTenant
    {
		public int? TenantId { get; set; }
			
		//[Required]
		[StringLength(VoucherConsts.MaxKeyLength, MinimumLength = VoucherConsts.MinKeyLength)]
		public virtual string Key { get; set; }
		
		public virtual decimal Value { get; set; }
		
		public virtual DateTime Expiry { get; set; }

        [Range(VoucherConsts.MinNoOfUsesValue, VoucherConsts.MaxNoOfUsesValue)]
        public virtual int? NoOfUses { get; set; } = 1;
		
		[StringLength(VoucherConsts.MaxDescriptionLength, MinimumLength = VoucherConsts.MinDescriptionLength)]
		public virtual string Description { get; set; }
		
		//[Required]
		[StringLength(VoucherConsts.MaxDiscountTypeLength, MinimumLength = VoucherConsts.MinDiscountTypeLength)]
		public virtual string DiscountType { get; set; }

        [NotMapped]
        public List<CreateOrEditVoucherEntityDto> VoucherEntities { get; set; } = new List<CreateOrEditVoucherEntityDto>();
    }
}