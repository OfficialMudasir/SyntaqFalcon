using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class GetAllProjectEnvironmentsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string NameFilter { get; set; }

        public string DescriptionFilter { get; set; }

        public int? EnvironmentTypeFilter { get; set; }

    }
}