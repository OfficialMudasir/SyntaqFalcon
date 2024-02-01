using Syntaq.Falcon.Records.Dtos;
using System;
using System.Collections.Generic;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Submissions
{
	public class SubmissionViewViewModel
	{
		public Guid Id { get; set; }
		public DateTime CreationTime { get; set; }
		public DateTime? LastModified { get; set; }
		public string UserName { get; set; }
		public string UserEmail { get; set; }
		public string Type { get; set; }
		public string SubmissionStatus { get; set; }
		public string RecordName { get; set; }
		public string RecordMatterName { get; set; }
		public List<RecordMatterItemDto> RecordMatterItems { get; set; }
		public bool RequiresPayment { get; set; }
		public string PaymentCurrency { get; set; }
		public string PaymentStatus { get; set; }
		public decimal? PaymentAmount { get; set; }
		public decimal? VoucherAmount { get; set; }
		public decimal? AmountPaid { get; set; }
		public string ChargeId { get; set; }
	}
}