
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Users.Dtos
{
    public class CreateOrEditUserAcceptanceTypeDto : EntityDto<Guid?>
    {

		[StringLength(UserAcceptanceTypeConsts.MaxNameLength, MinimumLength = UserAcceptanceTypeConsts.MinNameLength)]
		public string Name { get; set; }
		
		
		public bool Active { get; set; }

		[Required]
		public Guid? TemplateId { get; set; }
		 
		 
    }
}