using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.ProjectTemplates.Dtos
{
    public class GetAllProjectTemplatesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string NameFilter { get; set; }

        public string DescriptionFilter { get; set; }

    }
}
