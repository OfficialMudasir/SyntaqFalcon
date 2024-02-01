using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Syntaq.Falcon.DataExporting.Excel.NPOI;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Storage;

namespace Syntaq.Falcon.Records.Exporting
{
    public class RecordMatterContributorsExcelExporter : NpoiExcelExporterBase, IRecordMatterContributorsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public RecordMatterContributorsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetRecordMatterContributorForViewDto> recordMatterContributors)
        {
            return CreateExcelPackage(
                "RecordMatterContributors.xlsx",
                excelPackage =>
                {
                    
                    var sheet = excelPackage.CreateSheet(L("RecordMatterContributors"));

                    AddHeader(
                        sheet,
                        L("OrganizationName"),
                        L("Name"),
                        L("AccessToken"),
                        L("Time"),
                        L("StepStatus"),
                        L("StepRole"),
                        L("StepAction"),
                        L("Complete"),
                        L("Email"),
                        L("FormSchema"),
                        L("FormScript"),
                        L("FormRules"),
                        L("FormPages"),
                        (L("RecordMatter")) + L("RecordMatterName"),
                        (L("User")) + L("Name"),
                        (L("Form")) + L("Name")
                        );

                    AddObjects(
                        sheet, recordMatterContributors,
                        _ => _.RecordMatterContributor.OrganizationName,
                        _ => _.RecordMatterContributor.Name,
                        _ => _.RecordMatterContributor.AccessToken,
                        _ => _timeZoneConverter.Convert(_.RecordMatterContributor.Time, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.RecordMatterContributor.StepStatus,
                        _ => _.RecordMatterContributor.StepRole,
                        _ => _.RecordMatterContributor.StepAction,
                        _ => _.RecordMatterContributor.Complete,
                        _ => _.RecordMatterContributor.Email,
                        _ => _.RecordMatterContributor.FormSchema,
                        _ => _.RecordMatterContributor.FormScript,
                        _ => _.RecordMatterContributor.FormRules,
                        _ => _.RecordMatterContributor.FormPages,
                        _ => _.RecordMatterRecordMatterName,
                        _ => _.UserName,
                        _ => _.FormName
                        );

					
					for (var i = 1; i <= recordMatterContributors.Count; i++)
                    {
                        SetCellDataFormat(sheet.GetRow(i).Cells[4], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(4);
                });
        }
    }
}
