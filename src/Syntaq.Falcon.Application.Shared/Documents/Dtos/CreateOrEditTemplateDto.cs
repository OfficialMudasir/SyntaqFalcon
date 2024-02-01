
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Documents.Dtos
{
	public class CreateOrEditTemplateDto : EntityDto<Guid?>
	{
		public Guid OriginalId { get; set; }
		
		[StringLength(TemplateConsts.MaxNameLength, MinimumLength = TemplateConsts.MinNameLength)]
		public string Name { get; set; }
		public string DocumentName { get; set; }
		public byte[] Document { get; set; }
		
		public int Version { get; set; }
		
		public int CurrentVersion { get; set; }
		
		[StringLength(TemplateConsts.MaxCommentsLength, MinimumLength = TemplateConsts.MinCommentsLength)]
		public string Comments { get; set; }

		[Required]
		public Guid FolderId { get; set; }

        public int? TenantId { get; set; }
        public bool LockToTenant { get; set; }
	}
}