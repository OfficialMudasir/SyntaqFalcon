using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Syntaq.Falcon.DataExporting.Excel.NPOI;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Storage;

namespace Syntaq.Falcon.Records.Exporting
{
    public class RecordMatterAuditsExcelExporter : NpoiExcelExporterBase, IRecordMatterAuditsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public RecordMatterAuditsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetRecordMatterAuditForViewDto> recordMatterAudits)
        {
            return CreateExcelPackage(
                "RecordMatterAudits.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("RecordMatterAudits"));

                    AddHeader(
                        sheet,
                        L("Status"),
                        L("Data"),
                        (L("User")) + L("Name"),
                        (L("RecordMatter")) + L("RecordMatterName")
                        );

                    AddObjects(
                        sheet, recordMatterAudits,
                        _ => _.RecordMatterAudit.Status,
                        _ => _.RecordMatterAudit.Data,
                        _ => _.UserName,
                        _ => _.RecordMatterRecordMatterName
                        );

                });
        }
    }
}