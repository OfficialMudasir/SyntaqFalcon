using Syntaq.Falcon.VoucherEntitites.Dtos;
using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.VoucherEntities
{
    public class CreateOrEditVoucherEntityModalViewModel
    {
       public CreateOrEditVoucherEntityDto VoucherEntity { get; set; }

	   		public string VoucherTenantId { get; set;}


	   public bool IsEditMode => VoucherEntity.Id.HasValue;
    }
}