using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Forms.Dtos
{
	public class FormDto : EntityDto<Guid>
	{
		public Guid? RecordMatterId { get; set; }
		public string Description { get; set; }
		public string Name { get; set; }
		public string VersionName { get; set; }

		public bool PaymentEnabled { get; set; }
		public decimal PaymentAmount { get; set; }
		public string PaymentCurrency { get; set; }
		public string PaymentProcess { get; set; }
		public string PaymentProvider { get; set; }

		public Guid? FolderId { get; set; }
		public Guid? AppJobId { get; set; }
		public string Schema { get; set; }
		public string Script { get; set; }
		public List<FormRuleDto> Rules { get; set; } = new List<FormRuleDto>();
		public string RulesScript { get; set; }
 
		public virtual Guid OriginalId { get; set; }
		public int Version { get; set; }
		public int CurrentVersion { get; set; }
		public DateTime CreationTime { get; set; }
		public bool IsEnabled { get; set; }
		public bool ReadOnly { get; set; }
        public bool LockOnBuild { get; set; }
        
    }


}