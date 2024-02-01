using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.MergeTexts.Dtos
{
	public class CreateOrEditMergeTextItemDto : EntityDto<long?>
	{
		[Required]
		[StringLength(MergeTextItemConsts.MaxNameLength, MinimumLength = MergeTextItemConsts.MinNameLength)]
		public string Name { get; set; }
		public long? MergeTextId { get; set; }
		public List<CreateOrEditMergeTextItemValueDto> MTextList { get; set; }
	}
}