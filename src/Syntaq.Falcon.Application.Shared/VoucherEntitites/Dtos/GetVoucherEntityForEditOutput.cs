using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.VoucherEntitites.Dtos
{
    public class GetVoucherEntityForEditOutput
    {
		public CreateOrEditVoucherEntityDto VoucherEntity { get; set; }

		public string VoucherTenantId { get; set;}


    }
}