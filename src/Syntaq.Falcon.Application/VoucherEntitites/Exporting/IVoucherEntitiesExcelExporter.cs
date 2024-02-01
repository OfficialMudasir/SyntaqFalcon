using System.Collections.Generic;
using Syntaq.Falcon.VoucherEntitites.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.VoucherEntitites.Exporting
{
    public interface IVoucherEntitiesExcelExporter
    {
        FileDto ExportToFile(List<GetVoucherEntityForViewDto> voucherEntities);
    }
}