using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.VoucherUsages.Dtos
{
    public class GetVoucherUsageForEditOutput
    {
		public CreateOrEditVoucherUsageDto VoucherUsage { get; set; }

		public string UserName { get; set;}


    }
}