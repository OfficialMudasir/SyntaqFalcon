using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Syntaq.Falcon.DataExporting.Excel.EpPlus;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Forms.Dtos;
using Syntaq.Falcon.Storage;
using System.Collections.Generic;

namespace Syntaq.Falcon.Forms.Exporting
{
	public class FormsExcelExporter : EpPlusExcelExporterBase, IFormsExcelExporter
	{

		private readonly ITimeZoneConverter _timeZoneConverter;
		private readonly IAbpSession _abpSession;

		public FormsExcelExporter(
			ITimeZoneConverter timeZoneConverter,
			IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) : base(tempFileCacheManager)
		{
			_timeZoneConverter = timeZoneConverter;
			_abpSession = abpSession;
		}

		public FileDto ExportToFile(List<GetFormForView> forms)
		{
			return CreateExcelPackage(
				"Forms.xlsx",
				excelPackage =>
				{
					var sheet = excelPackage.Workbook.Worksheets.Add(L("Forms"));
					sheet.OutLineApplyStyle = true;

					AddHeader(
						sheet,
						L("Description"),
						//L("DocPDF"),
						//L("DocUserCanSave"),
						//L("DocWord"),
						//L("DocWordPaid"),
						L("Name"),
						L("PaymentAmount"),
						L("PaymentCurr"),
						L("PaymentEnabled"),
						L("Name")
						);

					AddObjects(
						sheet, 2, forms,
						_ => _.Form.Description,
						//_ => _.Form.DocPDF,
						//_ => _.Form.DocUserCanSave,
						//_ => _.Form.DocWord,
						//_ => _.Form.DocWordPaid,
						_ => _.Form.Name,
						_ => _.Form.PaymentAmount,
						_ => _.Form.PaymentCurrency,
						_ => _.Form.PaymentEnabled
						);

					

				});
		}
	}
}
