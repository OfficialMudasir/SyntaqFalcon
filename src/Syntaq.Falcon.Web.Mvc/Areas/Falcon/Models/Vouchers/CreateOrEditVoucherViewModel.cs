using Syntaq.Falcon.Vouchers.Dtos;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Vouchers
{
    public class CreateOrEditVoucherModalViewModel
    {
       public CreateOrEditVoucherDto Voucher { get; set; }

	   
	   public bool IsEditMode => Voucher.Id.HasValue;
    }
}