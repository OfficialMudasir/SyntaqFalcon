using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.MergeTexts.Dtos
{
    public class GetAllMergeTextsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string EntityTypeFilter { get; set; }

		public string EntityKeyFilter { get; set; }


		 public string MergeTextItemNameFilter { get; set; }

		 
    }
}