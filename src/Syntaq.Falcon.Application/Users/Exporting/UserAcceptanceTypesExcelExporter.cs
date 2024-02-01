using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Syntaq.Falcon.DataExporting.Excel.EpPlus;
using Syntaq.Falcon.Users.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Storage;

namespace Syntaq.Falcon.Users.Exporting
{
    public class UserAcceptanceTypesExcelExporter : EpPlusExcelExporterBase, IUserAcceptanceTypesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public UserAcceptanceTypesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetUserAcceptanceTypeForViewDto> userAcceptanceTypes)
        {
            return CreateExcelPackage(
                "UserAcceptanceTypes.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("UserAcceptanceTypes"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Name"),
                        L("Active"),
                        (L("Template")) + L("Name")
                        );

                    AddObjects(
                        sheet, 2, userAcceptanceTypes,
                        _ => _.UserAcceptanceType.Name,
                        _ => _.UserAcceptanceType.Active,
                        _ => _.TemplateName
                        );

					

                });
        }
    }
}
