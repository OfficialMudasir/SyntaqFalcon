
using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.MergeTexts.Dtos
{
	public class MergeTextDto : EntityDto<long>
	{
		public string EntityType { get; set; }

		public string EntityKey { get; set; }

		//public long? MergeTextItemId { get; set; }
	}
}