using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.MergeTexts.Dtos
{
	public class MergeTextItemDto : EntityDto<long>
	{
		public string Name { get; set; }
		public long? MergeTextItemValueId { get; set; }
	}
}