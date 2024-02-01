using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class GetProjectReleaseForEditOutput
    {
        public CreateOrEditProjectReleaseDto ProjectRelease { get; set; }

        public string ProjectEnvironmentName { get; set; }

        public bool HasDeployments { get; set; }

    }
}