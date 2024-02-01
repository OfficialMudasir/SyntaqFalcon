using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Forms.Dtos
{
	public class GetSystemFormForEdit : EntityDto<Guid?>
	{
		public virtual Guid? OriginalId { get; set; }
		public string Version { get; set; }
		public int FormType { get; set; }
	}
}