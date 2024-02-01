
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.VoucherEntitites.Dtos
{
    public class CreateOrEditVoucherEntityDto : EntityDto<Guid?>
    {
		public Guid EntityKey { get; set; }
		
		[Required]
		[StringLength(VoucherEntityConsts.MaxEntityTypeLength, MinimumLength = VoucherEntityConsts.MinEntityTypeLength)]
		public string EntityType { get; set; }

        [StringLength(VoucherEntityConsts.MaxEntityTypeLength)]
        public virtual string EntityName { get; set; }

        public Guid VoucherId { get; set; }
    }
}