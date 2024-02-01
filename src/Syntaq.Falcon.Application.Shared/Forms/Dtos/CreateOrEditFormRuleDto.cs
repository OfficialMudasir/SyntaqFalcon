using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Forms.Dtos
{
    public class CreateOrEditFormRuleDto : AuditedEntityDto<Guid?>
    {
        public int? TenantId { get; set; }
        [Required]
		public string Rule { get; set; }        

        public Guid FormId { get; set; } 
    }
}