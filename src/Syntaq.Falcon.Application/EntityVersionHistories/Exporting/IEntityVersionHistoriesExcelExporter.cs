using System.Collections.Generic;
using Syntaq.Falcon.EntityVersionHistories.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.EntityVersionHistories.Exporting
{
    public interface IEntityVersionHistoriesExcelExporter
    {
        FileDto ExportToFile(List<GetEntityVersionHistoryForViewDto> entityVersionHistories);
    }
}