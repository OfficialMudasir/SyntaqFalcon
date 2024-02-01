using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Syntaq.Falcon.DataExporting.Excel.EpPlus;
using Syntaq.Falcon.Users.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Storage;

namespace Syntaq.Falcon.Users.Exporting
{
    public class UserAcceptancesExcelExporter : EpPlusExcelExporterBase, IUserAcceptancesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public UserAcceptancesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetUserAcceptanceForViewDto> userAcceptances)
        {
            return CreateExcelPackage(
                "UserAcceptances.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("UserAcceptances"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        (L("UserAcceptanceType")) + L("Name"),
                        (L("User")) + L("Name"),
                        (L("User")) + L("Surname"),
                        L("EmailAddress"),
                        L("AcceptanceTime")
                        );

                    AddObjects(
                        sheet, 2, userAcceptances,
                        _ => _.UserAcceptanceTypeName,
                        _ => _.UserName,
                        _ => _.UserSurname,
                        _ => _.UserEmailAddress,
                        _ => _.UserAcceptance.CreationTime.ToLocalTime()
                        ); 
                });
        }
    }
}
