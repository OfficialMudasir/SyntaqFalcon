using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Syntaq.Falcon.DataExporting.Excel.NPOI;
using Syntaq.Falcon.RecordPolicies.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Storage;

namespace Syntaq.Falcon.RecordPolicies.Exporting
{
    public class RecordPoliciesExcelExporter : NpoiExcelExporterBase, IRecordPoliciesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public RecordPoliciesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetRecordPolicyForViewDto> recordPolicies)
        {
            return CreateExcelPackage(
                "RecordPolicies.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("RecordPolicies"));

                    AddHeader(
                        sheet,
                        L("Name"),
                        L("AppliedTenantId")
                        );

                    AddObjects(
                        sheet, recordPolicies,
                        _ => _.RecordPolicy.Name,
                        _ => _.RecordPolicy.AppliedTenantId
                        );

                });
        }
    }
}