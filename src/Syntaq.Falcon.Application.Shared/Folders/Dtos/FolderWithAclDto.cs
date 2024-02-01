using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Folders.Dtos
{
	public class FolderWithAclDto : AuditedEntityDto<Guid>
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public DateTime LastModified { get; set; }

		public Guid ParentId { get; set; }

		public string Type { get; set; }

		public long ACLCreator { get; set; }

		public string Role { get; set; }
	}
}
