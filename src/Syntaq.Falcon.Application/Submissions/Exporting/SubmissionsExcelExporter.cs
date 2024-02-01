using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Syntaq.Falcon.DataExporting.Excel.EpPlus;
using Syntaq.Falcon.Submissions.Dtos;
using Syntaq.Falcon.Dto;
using Syntaq.Falcon.Storage;
using System;

namespace Syntaq.Falcon.Submissions.Exporting
{
	public class SubmissionsExcelExporter : EpPlusExcelExporterBase, ISubmissionsExcelExporter
	{

		private readonly ITimeZoneConverter _timeZoneConverter;
		private readonly IAbpSession _abpSession;

		public SubmissionsExcelExporter( ITimeZoneConverter timeZoneConverter, IAbpSession abpSession, ITempFileCacheManager tempFileCacheManager) : base(tempFileCacheManager) { _timeZoneConverter = timeZoneConverter; _abpSession = abpSession; }

		public FileDto ExportToFile(List<GetSubmissionForViewDto> submissions)
		{
			return CreateExcelPackage(
				"Submissions.xlsx",
				excelPackage =>
				{
					var sheet = excelPackage.Workbook.Worksheets.Add(L("Submissions"));
					sheet.OutLineApplyStyle = true;

					AddHeader(
						sheet,
						L("Date"),
						L("AccessToken"),
						L("SubmissionStatus"),
						L("Type"),
						L("FormName"),
						L("AppJobName"),
						L("RecordName"),
						L("RecordMatterName"),
						L("RecordMatterItemCount"),
						L("UserName"),
						L("UserEmail"),
						L("RequiresPayment"),
						L("PaymentCurrency"),
						L("PaymentStatus"),
						L("PaymentAmount"),
						L("VoucherAmount"), 
						L("AmountPaid"),
						L("ChargeId")
						);

					AddObjects(
						sheet, 2, submissions,
						_ => _.Submission.Date.ToString("dd/MM/yyyy hh:mm tt"),
						_ => _.Submission.AccessToken,
						_ => _.Submission.SubmissionStatus,
						_ => _.Submission.Type,
						_ => _.FormName,
						_ => _.AppJobName,
						_ => _.RecordName,
						_ => _.RecordMatterName,
						_ => _.RecordMatterItemCount,
						_ => _.UserName,
						_ => _.UserEmail,
						_ => _.Submission.RequiresPayment == true ? "True" : "False",
						_ => _.Submission.PaymentCurrency,
						_ => _.Submission.PaymentStatus,
						_ => _.Submission.PaymentAmount != null ? decimal.Round((decimal)_.Submission.PaymentAmount, 2, MidpointRounding.AwayFromZero).ToString() : "0.00",
						_ => _.Submission.VoucherAmount != null ? decimal.Round((decimal)_.Submission.VoucherAmount, 2, MidpointRounding.AwayFromZero).ToString() : "0.00",
						_ => _.Submission.AmountPaid != null ? decimal.Round((decimal)_.Submission.AmountPaid, 2, MidpointRounding.AwayFromZero).ToString() : "0.00",
						_ => _.Submission.ChargeId
						);
				});
		}
	}
}
