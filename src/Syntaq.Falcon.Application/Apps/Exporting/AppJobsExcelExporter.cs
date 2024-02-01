using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Syntaq.Falcon.DataExporting.Excel.EpPlus;
using Syntaq.Falcon.Apps.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Storage;
using Abp.Runtime.Caching;
using System.Text.RegularExpressions;

namespace Syntaq.Falcon.Apps.Exporting
{
    public class AppJobsExcelExporter : EpPlusExcelExporterBase, IAppJobsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public AppJobsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetAppJobForView> appJobs)
        {
            return CreateExcelPackage(
                "AppJobs.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("AppJobs"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Name"),
                        (L("App")) + L("Name")
                        );

                    AddObjects(
                        sheet, 2, appJobs,
                        _ => Regex.Replace(_.AppJob.Name, @"[=()!+-/*^%@]", "x"),
                        _ => Regex.Replace(_.AppName, @"[=()!+-/*^%@]", "x")
                        );

                    //_ => string.Concat(_.AppJob.Name.Split(this.BAD_CHARS, StringSplitOptions.RemoveEmptyEntries)) ,

                });
        }

       
    }
}
