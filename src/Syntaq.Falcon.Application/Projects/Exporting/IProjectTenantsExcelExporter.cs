﻿using System.Collections.Generic;
using Syntaq.Falcon.Projects.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Projects.Exporting
{
    public interface IProjectTenantsExcelExporter
    {
        FileDto ExportToFile(List<GetProjectTenantForViewDto> projectTenants);
    }
}