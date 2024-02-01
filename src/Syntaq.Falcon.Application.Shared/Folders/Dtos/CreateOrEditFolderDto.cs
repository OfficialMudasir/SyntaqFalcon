using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Folders.Dtos
{
    public class CreateOrEditFolderDto : AuditedEntityDto<Guid?>
    {
        [Required]
		[StringLength(FolderConsts.MaxNameLength, MinimumLength = FolderConsts.MinNameLength)]
		public string Name { get; set; }
				
		[StringLength(FolderConsts.MaxDescriptionLength, MinimumLength = FolderConsts.MinDescriptionLength)]
		public string Description { get; set; }

        public Guid ParentId { get; set; }
		
		public string Type { get; set; }

    }
}