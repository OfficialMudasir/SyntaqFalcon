using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class GetProjectDeploymentForEditOutput
    {
        public CreateOrEditProjectDeploymentDto ProjectDeployment { get; set; }

        public string ProjectReleaseName { get; set; }
        public string ProjectReleaseNotes { get; set; }

    }
}