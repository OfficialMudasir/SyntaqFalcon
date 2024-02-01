using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Apps.Dtos
{
	public class GetAllAppsInput : PagedAndSortedResultRequestDto
	{
		public string Filter { get; set; }

		public string NameFilter { get; set; }

		public string DescriptionFilter { get; set; }			   
	}
}