using System.Collections.Generic;
using Syntaq.Falcon.Submissions.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Submissions.Exporting
{
    public interface ISubmissionsExcelExporter
    {
        FileDto ExportToFile(List<GetSubmissionForViewDto> submissions);
    }
}