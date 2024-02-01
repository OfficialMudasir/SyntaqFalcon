using System.Collections.Generic;
using Syntaq.Falcon.Vouchers.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Vouchers.Exporting
{
    public interface IVouchersExcelExporter
    {
        FileDto ExportToFile(List<GetVoucherForViewDto> vouchers);
    }
}