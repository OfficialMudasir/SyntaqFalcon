using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Syntaq.Falcon.DataExporting.Excel.NPOI;
using Syntaq.Falcon.Projects.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Storage;

namespace Syntaq.Falcon.Projects.Exporting
{
    public class ProjectDeploymentsExcelExporter : NpoiExcelExporterBase, IProjectDeploymentsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ProjectDeploymentsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetProjectDeploymentForViewDto> projectDeployments)
        {
            return CreateExcelPackage(
                "ProjectDeployments.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("ProjectDeployments"));

                    AddHeader(
                        sheet,
                        L("Comments"),
                        L("ActionType"),
                        (L("ProjectRelease")) + L("Name")
                        );

                    AddObjects(
                        sheet, projectDeployments,
                        _ => _.ProjectDeployment.Comments,
                        _ => _.ProjectDeployment.ActionType,
                        _ => _.ProjectReleaseName
                        );

                });
        }
    }
}