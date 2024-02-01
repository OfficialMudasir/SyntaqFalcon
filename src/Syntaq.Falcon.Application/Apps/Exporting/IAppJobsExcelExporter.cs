using System.Collections.Generic;
using Syntaq.Falcon.Apps.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Apps.Exporting
{
    public interface IAppJobsExcelExporter
    {
        FileDto ExportToFile(List<GetAppJobForView> appJobs);
    }
}