using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Syntaq.Falcon.DataExporting.Excel.EpPlus;
using Syntaq.Falcon.VoucherEntitites.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Storage;

namespace Syntaq.Falcon.VoucherEntitites.Exporting
{
    public class VoucherEntitiesExcelExporter : EpPlusExcelExporterBase, IVoucherEntitiesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public VoucherEntitiesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetVoucherEntityForViewDto> voucherEntities)
        {
            return CreateExcelPackage(
                "VoucherEntities.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("VoucherEntities"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("EntityKey"),
                        L("EntityType"),
                        (L("Voucher")) + L("TenantId")
                        );

                    AddObjects(
                        sheet, 2, voucherEntities,
                        _ => _.VoucherEntity.EntityKey,
                        _ => _.VoucherEntity.EntityType,
                        _ => _.VoucherTenantId
                        );

					

                });
        }
    }
}
