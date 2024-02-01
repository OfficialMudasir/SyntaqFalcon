using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class GetProjectEnvironmentForEditOutput
    {
        public CreateOrEditProjectEnvironmentDto ProjectEnvironment { get; set; }

    }
}