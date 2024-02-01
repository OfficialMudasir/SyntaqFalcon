using System.Collections.Generic;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.Records.Exporting
{
    public interface IRecordMatterContributorsExcelExporter
    {
        FileDto ExportToFile(List<GetRecordMatterContributorForViewDto> recordMatterContributors);
    }
}