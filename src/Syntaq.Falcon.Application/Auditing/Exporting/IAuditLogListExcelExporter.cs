using System.Collections.Generic;
using Syntaq.Falcon.Auditing.Dto;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Auditing.Exporting
{
    public interface IAuditLogListExcelExporter
    {
        FileDto ExportToFile(List<AuditLogListDto> auditLogListDtos);

        FileDto ExportToFile(List<EntityChangeListDto> entityChangeListDtos);
    }
}
