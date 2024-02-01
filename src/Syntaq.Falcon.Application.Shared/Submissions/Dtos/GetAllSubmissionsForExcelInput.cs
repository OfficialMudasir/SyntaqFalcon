using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Submissions.Dtos
{
	public class GetAllSubmissionsForExcelInput
	{
		public string Filter { get; set; }
		public DateTime StartDateFilter { get; set; }
		public DateTime EndDateFilter { get; set; }
		public int RequiresPaymentFilter { get; set; }
		public string PaymentStatusFilter { get; set; }
		public string ChargeIdFilter { get; set; }
		public string SubmissionStatusFilter { get; set; }
		public string TypeFilter { get; set; }
		public string FormNameFilter { get; set; }
		public string RecordNameFilter { get; set; }
		public string RecordMatterNameFilter { get; set; }
		public string UserNameFilter { get; set; }
		public string AppJobNameFilter { get; set; }
		public bool ExcludeOwnAccountFilter { get; set; }
	}
}