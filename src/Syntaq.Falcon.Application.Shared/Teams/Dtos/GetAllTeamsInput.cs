using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Teams.Dtos
{
    public class GetAllTeamsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string TeamNameFilter { get; set; }



    }
}