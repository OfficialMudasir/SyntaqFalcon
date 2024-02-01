using System.Collections.Generic;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Records.Exporting
{
    public interface IRecordMatterAuditsExcelExporter
    {
        FileDto ExportToFile(List<GetRecordMatterAuditForViewDto> recordMatterAudits);
    }
}