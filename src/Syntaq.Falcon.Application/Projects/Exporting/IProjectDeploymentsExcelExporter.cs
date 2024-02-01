using System.Collections.Generic;
using Syntaq.Falcon.Projects.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Projects.Exporting
{
    public interface IProjectDeploymentsExcelExporter
    {
        FileDto ExportToFile(List<GetProjectDeploymentForViewDto> projectDeployments);
    }
}