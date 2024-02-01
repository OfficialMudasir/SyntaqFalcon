using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Syntaq.Falcon.DataExporting.Excel.EpPlus;
using Syntaq.Falcon.Vouchers.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Storage;

namespace Syntaq.Falcon.Vouchers.Exporting
{
    public class VouchersExcelExporter : EpPlusExcelExporterBase, IVouchersExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public VouchersExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetVoucherForViewDto> vouchers)
        {
            return CreateExcelPackage(
                "Vouchers.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Vouchers"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Key"),
                        L("Value"),
                        L("Expiry"),
                        L("NoOfUses"),
                        L("Description"),
                        L("DiscountType")
                        );

                    AddObjects(
                        sheet, 2, vouchers,
                        _ => _.Voucher.Key,
                        _ => _.Voucher.Value,
                        _ => _timeZoneConverter.Convert(_.Voucher.Expiry, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.Voucher.NoOfUses,
                        _ => _.Voucher.Description,
                        _ => _.Voucher.DiscountType
                        );

					var expiryColumn = sheet.Column(3);
                    expiryColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					expiryColumn.AutoFit();
					

                });
        }
    }
}
