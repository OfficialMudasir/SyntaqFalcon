using Abp.Application.Services.Dto;
using Syntaq.Falcon.Projects.Dtos;
using Syntaq.Falcon.Tags.Dtos;
using System.Collections.Generic;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Projects
{
    public class ProjectsViewModel
    {
		public string FilterText { get; set; }
        public int? StatusFilter { get; set; }
        public IReadOnlyList<GetProjectForViewDto> Projects { get; set; }

        public PagedResultDto<GetTagEntityTypeForViewDto> Tags { get; set; }
    }
}