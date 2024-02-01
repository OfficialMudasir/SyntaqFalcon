
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.MergeTexts.Dtos
{
	public class CreateOrEditMergeTextItemValueDto : EntityDto<long?>
	{
		[StringLength(MergeTextItemValueConsts.MaxKeyLength, MinimumLength = MergeTextItemValueConsts.MinKeyLength)]
		public string Key { get; set; }
		
		[StringLength(MergeTextItemValueConsts.MaxValueLength, MinimumLength = MergeTextItemValueConsts.MinValueLength)]
		public string Value { get; set; }

		public long MergeTextItemId { get; set; }
	}
}