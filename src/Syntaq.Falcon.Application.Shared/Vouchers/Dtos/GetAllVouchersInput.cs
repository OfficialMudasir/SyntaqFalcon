using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Vouchers.Dtos
{
    public class GetAllVouchersInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string KeyFilter { get; set; }

		public DateTime? MaxExpiryFilter { get; set; }
		public DateTime? MinExpiryFilter { get; set; }

		public int? MaxNoOfUsesFilter { get; set; }
		public int? MinNoOfUsesFilter { get; set; }

		public string DiscountTypeFilter { get; set; }



    }
}