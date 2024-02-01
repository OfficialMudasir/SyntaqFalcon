
using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Documents.Dtos
{
	public class TemplateDto : EntityDto<Guid>
	{
		public string Name { get; set; }
		public string DocumentName { get; set; }
		public int Version { get; set; }
		public int CurrentVersion { get; set; }
		public string Comments { get; set; }
		public Guid OriginalId { get; set; }
		public string Type { get; set; }
		public string UserACLPermission { get; set; }
		public DateTime CreationTime { get; set; }
		public long? CreatorUserId { get; set; }
	}
}