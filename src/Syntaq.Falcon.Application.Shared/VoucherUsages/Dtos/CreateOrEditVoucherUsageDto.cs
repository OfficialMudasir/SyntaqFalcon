
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.VoucherUsages.Dtos
{
	public class CreateOrEditVoucherUsageDto : EntityDto<Guid?>
	{
		   
		public int? TenantId { get; set; }
		//public DateTime DateRedeemed { get; set; }
		
		[Required]
		[StringLength(VoucherUsageConsts.MaxEntityKeyLength, MinimumLength = VoucherUsageConsts.MinEntityKeyLength)]
		public string EntityKey { get; set; }
		
		
		[Required]
		[StringLength(VoucherUsageConsts.MaxEntityTypeLength, MinimumLength = VoucherUsageConsts.MinEntityTypeLength)]
		public string EntityType { get; set; }
		
		
		 //public long UserId { get; set; }
		 
		 public Guid VoucherId { get; set; }
		 
	}
}