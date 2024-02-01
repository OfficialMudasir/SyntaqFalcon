using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.VoucherUsages.Dtos
{
    public class GetAllVoucherUsagesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public DateTime? MaxDateRedeemedFilter { get; set; }
		public DateTime? MinDateRedeemedFilter { get; set; }

		public string EntityKeyFilter { get; set; }

		public string EntityTypeFilter { get; set; }


		 public string UserNameFilter { get; set; }

		 
    }
}