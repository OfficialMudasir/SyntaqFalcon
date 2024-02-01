
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Submissions.Dtos
{
	public class CreateOrEditSubmissionDto : FullAuditedEntityDto<Guid?>
	{
		public int? TenantId { get; set; }
		public string AccessToken { get; set; }
		public bool RequiresPayment { get; set; }
		public string PaymentCurrency { get; set; }
		public string PaymentStatus { get; set; }
		public decimal? PaymentAmount { get; set; }
		public decimal? VoucherAmount { get; set; }
		public decimal? AmountPaid { get; set; }
		public string ChargeId { get; set; }
		[Required]
		public string SubmissionStatus { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
		public Guid? RecordId { get; set; }
		public Guid? RecordMatterId { get; set; }
		public long? UserId { get; set; }
		public Guid? AppId { get; set; }
		public Guid? AppJobId { get; set; }
		public Guid? FormId { get; set; }
	}
}