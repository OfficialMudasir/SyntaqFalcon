using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Vouchers.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public Guid ParentId { get; set; }
		public string Filter { get; set; }
    }

    public class GetVoucherDetailsByKeyInput  
    {

        public string VoucherKey { get; set; }
        public Guid? EntityId { get; set; }
        public string Balance { get; set; }
    }

}