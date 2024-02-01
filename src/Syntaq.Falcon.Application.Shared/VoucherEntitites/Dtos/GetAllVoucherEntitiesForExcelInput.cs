using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.VoucherEntitites.Dtos
{
    public class GetAllVoucherEntitiesForExcelInput
    {
		public string Filter { get; set; }

		public Guid? EntityKeyFilter { get; set; }

		public string EntityTypeFilter { get; set; }


		 public int VoucherTenantIdFilter { get; set; }

		 
    }
}