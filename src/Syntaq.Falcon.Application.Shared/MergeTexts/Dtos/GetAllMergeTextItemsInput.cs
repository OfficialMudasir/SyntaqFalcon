using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.MergeTexts.Dtos
{
    public class GetAllMergeTextItemsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string NameFilter { get; set; }


		 public string MergeTextItemValueKeyFilter { get; set; }

		 
    }
}