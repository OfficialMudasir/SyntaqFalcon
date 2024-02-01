using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Syntaq.Falcon.DataExporting.Excel.NPOI;
using Syntaq.Falcon.RecordPolicyActions.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Storage;

namespace Syntaq.Falcon.RecordPolicyActions.Exporting
{
    public class RecordPolicyActionsExcelExporter : NpoiExcelExporterBase, IRecordPolicyActionsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public RecordPolicyActionsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetRecordPolicyActionForViewDto> recordPolicyActions)
        {
            return CreateExcelPackage(
                "RecordPolicyActions.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("RecordPolicyActions"));

                    AddHeader(
                        sheet,
                        L("Name"),
                        L("AppliedTenantId"),
                        L("ExpireDays"),
                        L("Active"),
                        L("Type"),
                        L("RecordStatus"),
                        (L("RecordPolicy")) + L("Name")
                        );

                    AddObjects(
                        sheet, recordPolicyActions,
                        _ => _.RecordPolicyAction.Name,
                        _ => _.RecordPolicyAction.AppliedTenantId,
                        _ => _.RecordPolicyAction.ExpireDays,
                        _ => _.RecordPolicyAction.Active,
                        _ => _.RecordPolicyAction.Type,
                        _ => _.RecordPolicyAction.RecordStatus
                        );

                });
        }
    }
}