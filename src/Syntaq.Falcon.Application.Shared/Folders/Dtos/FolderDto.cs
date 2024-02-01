using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Folders.Dtos
{
	public class FolderDto : AuditedEntityDto<Guid>
	{
		public string Name { get; set; }
		public Guid? ParentId { get; set; }
		public string Description { get; set; }

		public DateTime LastModified { get; set; }

		public string Type { get; set; }

		public string UserACLPermission { get; set; }
	}
}