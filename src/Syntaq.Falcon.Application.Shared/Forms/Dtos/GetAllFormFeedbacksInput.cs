using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Forms.Dtos
{
    public class GetAllFormFeedbacksInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }


		 public string FormNameFilter { get; set; }

		 
    }
}