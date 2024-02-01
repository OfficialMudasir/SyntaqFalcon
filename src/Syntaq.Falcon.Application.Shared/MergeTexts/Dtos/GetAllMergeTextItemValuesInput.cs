using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.MergeTexts.Dtos
{
    public class GetAllMergeTextItemValuesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string KeyFilter { get; set; }

		public string ValueFilter { get; set; }



    }
}