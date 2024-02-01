using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Syntaq.Falcon.DataExporting.Excel.EpPlus;
using Syntaq.Falcon.VoucherUsages.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Storage;

namespace Syntaq.Falcon.VoucherUsages.Exporting
{
    public class VoucherUsagesExcelExporter : EpPlusExcelExporterBase, IVoucherUsagesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public VoucherUsagesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetVoucherUsageForViewDto> voucherUsages)
        {
            return CreateExcelPackage(
                "VoucherUsages.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("VoucherUsages"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("DateRedeemed"),
                        L("EntityKey"),
                        L("EntityType"),
                        (L("User")) + L("Name")
                        );

                    AddObjects(
                        sheet, 2, voucherUsages,
                        _ => _timeZoneConverter.Convert(_.VoucherUsage.DateRedeemed, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.VoucherUsage.EntityKey,
                        _ => _.VoucherUsage.EntityType,
                        _ => _.UserName
                        );

					var dateRedeemedColumn = sheet.Column(1);
                    dateRedeemedColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					dateRedeemedColumn.AutoFit();
					

                });
        }
    }
}
