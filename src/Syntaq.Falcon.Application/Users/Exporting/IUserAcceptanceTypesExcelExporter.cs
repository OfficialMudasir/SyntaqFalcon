using System.Collections.Generic;
using Syntaq.Falcon.Users.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Users.Exporting
{
    public interface IUserAcceptanceTypesExcelExporter
    {
        FileDto ExportToFile(List<GetUserAcceptanceTypeForViewDto> userAcceptanceTypes);
    }
}