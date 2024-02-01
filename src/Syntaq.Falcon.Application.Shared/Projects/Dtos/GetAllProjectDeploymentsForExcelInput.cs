using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class GetAllProjectDeploymentsForExcelInput
    {
        public string Filter { get; set; }

        public string CommentsFilter { get; set; }

        public int? ActionTypeFilter { get; set; }

        public string ProjectReleaseNameFilter { get; set; }

    }
}