
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using Syntaq.Falcon.VoucherEntitites.Dtos;
using System.Collections.Generic;

namespace Syntaq.Falcon.Vouchers.Dtos
{
    public class CreateOrEditVoucherDto : EntityDto<Guid?>
    {
        //public Guid ID { get; set; }

        //[Required]
        [StringLength(VoucherConsts.MaxKeyLength, MinimumLength = VoucherConsts.MinKeyLength)]
        public string Key { get; set; }


        public decimal Value { get; set; }


        public DateTime Expiry { get; set; } = DateTime.Today.AddDays(30);


        [Range(VoucherConsts.MinNoOfUsesValue, VoucherConsts.MaxNoOfUsesValue)]
        public int? NoOfUses { get; set; } = 1;


        [StringLength(VoucherConsts.MaxDescriptionLength, MinimumLength = VoucherConsts.MinDescriptionLength)]
        public string Description { get; set; }


        //[Required]
        [StringLength(VoucherConsts.MaxDiscountTypeLength, MinimumLength = VoucherConsts.MinDiscountTypeLength)]
        public string DiscountType { get; set; }

        public List<CreateOrEditVoucherEntityDto> VoucherEntities { get; set; } = new List<CreateOrEditVoucherEntityDto>();

    }
}