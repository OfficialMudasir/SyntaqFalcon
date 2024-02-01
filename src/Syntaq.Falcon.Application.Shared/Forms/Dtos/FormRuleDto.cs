
using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Forms.Dtos
{
    public class FormRuleDto : AuditedEntityDto<Guid?>
    {
        public int? TenantId { get; set; }

        public string Rule { get; set; }

		public bool IsEnabled { get; set; }
        
	    public Guid FormId { get; set; }		 
    }
}