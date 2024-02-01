
using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.VoucherUsages.Dtos
{
    public class VoucherUsageDto : EntityDto<Guid>
    {
		public DateTime DateRedeemed { get; set; }

		public string EntityKey { get; set; }

		public string EntityType { get; set; }


		 public long UserId { get; set; }

		 
    }
}