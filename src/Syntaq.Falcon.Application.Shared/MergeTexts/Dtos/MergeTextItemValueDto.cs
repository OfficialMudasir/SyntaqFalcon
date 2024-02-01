using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.MergeTexts.Dtos
{
	public class MergeTextItemValueDto : EntityDto<long>
	{
		public string Key { get; set; }
		public string Value { get; set; }
	}
}