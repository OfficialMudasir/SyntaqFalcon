
using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.VoucherEntitites.Dtos
{
    public class VoucherEntityDto : EntityDto<Guid>
    {
		public Guid EntityKey { get; set; }

		public string EntityType { get; set; }


		 public Guid VoucherId { get; set; }

		 
    }
}