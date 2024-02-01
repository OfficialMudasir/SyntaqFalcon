using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Syntaq.Falcon.DataExporting.Excel.NPOI;
using Syntaq.Falcon.Projects.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Storage;

namespace Syntaq.Falcon.Projects.Exporting
{
    public class ProjectReleasesExcelExporter : NpoiExcelExporterBase, IProjectReleasesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ProjectReleasesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetProjectReleaseForViewDto> projectReleases)
        {
            return CreateExcelPackage(
                "ProjectReleases.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("ProjectReleases"));

                    AddHeader(
                        sheet,
                        L("Name"),
                        L("Notes"),
                        L("ProjectId"),
                        L("Required"),
                        L("VersionMajor"),
                        L("VersionMinor"),
                        L("VersionRevision"),
                        L("ReleaseType"),
                        (L("ProjectEnvironment")) + L("Name")
                        );

                    AddObjects(
                        sheet, projectReleases,
                        _ => _.ProjectRelease.Name,
                        _ => _.ProjectRelease.Notes,
                        _ => _.ProjectRelease.ProjectId,
                        _ => _.ProjectRelease.Required,
                        _ => _.ProjectRelease.VersionMajor,
                        _ => _.ProjectRelease.VersionMinor,
                        _ => _.ProjectRelease.VersionRevision,
                        _ => _.ProjectRelease.ReleaseType,
                        _ => _.ProjectEnvironmentName
                        );

                });
        }
    }
}