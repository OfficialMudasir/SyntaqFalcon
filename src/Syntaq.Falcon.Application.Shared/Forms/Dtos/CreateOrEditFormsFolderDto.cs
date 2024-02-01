
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Forms.Dtos
{
    public class CreateOrEditFormsFolderDto : AuditedEntityDto<Guid?>
    {

		public Guid ParentId { get; set; }
		
		
		[Required]
		[StringLength(FormsFolderConsts.MaxNameLength, MinimumLength = FormsFolderConsts.MinNameLength)]
		public string Name { get; set; }
		
		
		[StringLength(FormsFolderConsts.MaxDescriptionLength, MinimumLength = FormsFolderConsts.MinDescriptionLength)]
		public string Description { get; set; }
		
		

    }
}