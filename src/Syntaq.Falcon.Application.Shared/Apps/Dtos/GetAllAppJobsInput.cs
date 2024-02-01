using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Apps.Dtos
{
    public class GetAllAppJobsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string NameFilter { get; set; }


		 public string AppNameFilter { get; set; }

		 
    }
}