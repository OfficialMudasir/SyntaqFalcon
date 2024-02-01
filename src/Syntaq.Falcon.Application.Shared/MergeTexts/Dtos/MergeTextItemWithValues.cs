using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.MergeTexts.Dtos
{
	public class MergeTextItemWithValues : EntityDto<long>
	{
		public string Name { get; set; }
		public List<MergeTextItemValueDto> ItemValues { get; set; }
	}
}
