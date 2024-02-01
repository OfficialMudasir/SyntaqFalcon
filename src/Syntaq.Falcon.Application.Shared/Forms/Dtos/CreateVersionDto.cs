using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Forms.Dtos
{
	public class CreateVersionDto : EntityDto<Guid>
	{
		public Guid OriginalId { get; set; }
		public string VersionName { get; set; }
		public string VersionDes { get; set; }
	}
}
