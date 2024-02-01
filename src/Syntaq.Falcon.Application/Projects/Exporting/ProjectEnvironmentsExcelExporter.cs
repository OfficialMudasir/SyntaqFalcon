using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Syntaq.Falcon.DataExporting.Excel.NPOI;
using Syntaq.Falcon.Projects.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Storage;

namespace Syntaq.Falcon.Projects.Exporting
{
    public class ProjectEnvironmentsExcelExporter : NpoiExcelExporterBase, IProjectEnvironmentsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ProjectEnvironmentsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetProjectEnvironmentForViewDto> projectEnvironments)
        {
            return CreateExcelPackage(
                "ProjectEnvironments.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("ProjectEnvironments"));

                    AddHeader(
                        sheet,
                        L("Name"),
                        L("Description"),
                        L("EnvironmentType")
                        );

                    AddObjects(
                        sheet, projectEnvironments,
                        _ => _.ProjectEnvironment.Name,
                        _ => _.ProjectEnvironment.Description,
                        _ => _.ProjectEnvironment.EnvironmentType
                        );

                });
        }
    }
}