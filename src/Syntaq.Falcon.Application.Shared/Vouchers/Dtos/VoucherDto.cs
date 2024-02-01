
using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Vouchers.Dtos
{
    public class VoucherDto : EntityDto<Guid>
    {
		public string Key { get; set; }

		public decimal Value { get; set; }

		public DateTime Expiry { get; set; }

		public int? NoOfUses { get; set; }

		public string Description { get; set; }

		public string DiscountType { get; set; }



    }
}