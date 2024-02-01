using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.Vouchers.Dtos
{
    public class VoucherValidityDto
    {
        public Guid? VoucherId { get; set; }
        public bool VoucherValid { get; set; }
        public string ValidityMessage { get; set; }
        public int? NoOfUses { get; set; }
        public decimal? VoucherValue { get; set; }
        public string DiscountType { get; set; }
    }
}
