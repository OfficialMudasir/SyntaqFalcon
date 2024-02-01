using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Syntaq.Falcon.DataExporting.Excel.NPOI;
using Syntaq.Falcon.EntityVersionHistories.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Storage;

namespace Syntaq.Falcon.EntityVersionHistories.Exporting
{
    public class EntityVersionHistoriesExcelExporter : NpoiExcelExporterBase, IEntityVersionHistoriesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public EntityVersionHistoriesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetEntityVersionHistoryForViewDto> entityVersionHistories)
        {
            return CreateExcelPackage(
                "EntityVersionHistories.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("EntityVersionHistories"));

                    AddHeader(
                        sheet,
                        L("Name"),
                        L("Version"),
                        L("PreviousVersion"),
                        L("Type"),
                        (L("User")) + L("Name")
                        );

                    AddObjects(
                        sheet, entityVersionHistories,
                        _ => _.EntityVersionHistory.Name,
                        _ => _.EntityVersionHistory.Version,
                        _ => _.EntityVersionHistory.PreviousVersion,
                        _ => _.EntityVersionHistory.Type,
                        _ => _.UserName
                        );

                });
        }
    }
}