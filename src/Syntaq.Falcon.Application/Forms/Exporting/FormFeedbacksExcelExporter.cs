using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Syntaq.Falcon.DataExporting.Excel.NPOI;
using Syntaq.Falcon.Forms.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Storage;

namespace Syntaq.Falcon.Forms.Exporting
{
    public class FormFeedbacksExcelExporter : NpoiExcelExporterBase, IFormFeedbacksExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public FormFeedbacksExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetFormFeedbackForViewDto> formFeedbacks)
        {
            return CreateExcelPackage(
                "FormFeedbacks.xlsx",
                excelPackage =>
                {
                    
                    var sheet = excelPackage.CreateSheet(L("FormFeedbacks"));

                    AddHeader(
                        sheet,
                        (L("Form")) + L("Name"),
                         L("Sender"),
                         L("Email"),
                         L("Rating"),
                         L("Comments"),
                         L("TimeReceived")
                        );

                    AddObjects(
                        sheet, formFeedbacks,
                        _ => _.FormName,
                        _ => _.UserName,
                        _ => _.Email,
                        _ => _.Rating,
                        _ => _.Comment,
                        _ => _.FormFeedback.CreationTime
                        );

					
					
                });
        }
    }
}
