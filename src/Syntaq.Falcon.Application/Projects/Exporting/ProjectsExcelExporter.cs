using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Syntaq.Falcon.DataExporting.Excel.NPOI;
using Syntaq.Falcon.Projects.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Storage;

namespace Syntaq.Falcon.Projects.Exporting
{
    public class ProjectsExcelExporter : NpoiExcelExporterBase, IProjectsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ProjectsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetProjectForViewDto> projects)
        {
            return CreateExcelPackage(
                "Projects.xlsx",
                excelPackage =>
                {
                    
                    var sheet = excelPackage.CreateSheet(L("Projects"));

                    AddHeader(
                        sheet,
                        L("Name"),
                        L("Description"),
                        L("Status"),
                        L("Type"),
                        (L("Record")) + L("RecordName")
                        );

                    AddObjects(
                        sheet, projects,
                        _ => _.Project.Name,
                        _ => _.Project.Description,
                        _ => _.Project.Status,
                        _ => _.Project.Type,
                        _ => _.RecordRecordName
                        );

					
					
                });
        }
    }
}
