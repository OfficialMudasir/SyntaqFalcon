using Syntaq.Falcon.VoucherUsages.Dtos;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.VoucherUsages
{
    public class CreateOrEditVoucherUsageModalViewModel
    {
       public CreateOrEditVoucherUsageDto VoucherUsage { get; set; }

	   		public string UserName { get; set;}


	   public bool IsEditMode => VoucherUsage.Id.HasValue;
    }
}