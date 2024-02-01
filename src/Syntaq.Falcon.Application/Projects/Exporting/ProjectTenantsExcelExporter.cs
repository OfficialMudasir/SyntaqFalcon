using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Syntaq.Falcon.DataExporting.Excel.NPOI;
using Syntaq.Falcon.Projects.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Storage;

namespace Syntaq.Falcon.Projects.Exporting
{
    public class ProjectTenantsExcelExporter : NpoiExcelExporterBase, IProjectTenantsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ProjectTenantsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetProjectTenantForViewDto> projectTenants)
        {
            return CreateExcelPackage(
                "ProjectTenants.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("ProjectTenants"));

                    AddHeader(
                        sheet,
                        L("SubscriberTenantId"),
                        L("ProjectId"),
                        L("Enabled"),
                        (L("ProjectEnvironment")) + L("Name")
                        );

                    AddObjects(
                        sheet, projectTenants,
                        _ => _.ProjectTenant.SubscriberTenantId,
                        _ => _.ProjectTenant.ProjectId,
                        _ => _.ProjectTenant.Enabled,
                        _ => _.ProjectEnvironmentName
                        );

                });
        }
    }
}