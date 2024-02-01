
using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Users.Dtos
{
    public class UserAcceptanceTypeDto : EntityDto<Guid>
    {

		public string Name { get; set; }
		public bool Active { get; set; }

		// Depreciated this name is unneccesarily long
		// If using templateId then use TemplateVersion
		public int CurrentAcceptanceDocTemplateVersion { get; set; } 

		public Guid? TemplateId { get; set; }
		public int TemplateVersion { get; set; }
		public String TemplateContent { get; set; }	
	}
}