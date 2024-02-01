using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.VoucherEntitites.Dtos
{
    public class GetAllVoucherEntitiesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public Guid? EntityKeyFilter { get; set; }

		public string EntityTypeFilter { get; set; }


		 public string VoucherTenantIdFilter { get; set; }

		 
    }
}