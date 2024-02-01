using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Vouchers.Dtos
{
    public class GetVoucherForEditOutput
    {
		public CreateOrEditVoucherDto Voucher { get; set; }


    }
}