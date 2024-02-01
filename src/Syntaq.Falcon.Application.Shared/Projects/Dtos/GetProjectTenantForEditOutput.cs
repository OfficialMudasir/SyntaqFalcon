using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class GetProjectTenantForEditOutput
    {
        public CreateOrEditProjectTenantDto ProjectTenant { get; set; }

        public string ProjectEnvironmentName { get; set; }


    }
}