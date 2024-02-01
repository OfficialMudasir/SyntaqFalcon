using System.Collections.Generic;
using Syntaq.Falcon.RecordPolicyActions.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.RecordPolicyActions.Exporting
{
    public interface IRecordPolicyActionsExcelExporter
    {
        FileDto ExportToFile(List<GetRecordPolicyActionForViewDto> recordPolicyActions);
    }
}