using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class GetAllProjectDeploymentsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string CommentsFilter { get; set; }

        public int? ActionTypeFilter { get; set; }

        public string ProjectReleaseNameFilter { get; set; }

        public Guid? ProjectId { get; set; }
        public Guid? ReleaseId { get; set; }
        public Guid? EnvironmentId { get; set; }

    }
}