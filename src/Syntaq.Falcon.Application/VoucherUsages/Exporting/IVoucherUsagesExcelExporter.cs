using System.Collections.Generic;
using Syntaq.Falcon.VoucherUsages.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.VoucherUsages.Exporting
{
    public interface IVoucherUsagesExcelExporter
    {
        FileDto ExportToFile(List<GetVoucherUsageForViewDto> voucherUsages);
    }
}