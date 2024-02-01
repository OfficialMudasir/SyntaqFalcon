using System.Collections.Generic;
using Syntaq.Falcon.RecordPolicies.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.RecordPolicies.Exporting
{
    public interface IRecordPoliciesExcelExporter
    {
        FileDto ExportToFile(List<GetRecordPolicyForViewDto> recordPolicies);
    }
}