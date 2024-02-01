using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Forms.Dtos
{
    public class GetAllFormsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

        public string Id { get; set; }

        public string Type { get; set; }

        public string DescriptionFilter { get; set; }

		public int DocPDFFilter { get; set; }

		public int DocUserCanSaveFilter { get; set; }

		public int DocWordFilter { get; set; }

		public int DocWordPaidFilter { get; set; }

		public string NameFilter { get; set; }

		public decimal? MaxPaymentAmountFilter { get; set; }
		public decimal? MinPaymentAmountFilter { get; set; }

		public string PaymentCurrFilter { get; set; }

		public int PaymentEnabledFilter { get; set; }

        public string FormsFolderNameFilter { get; set; }
        public Guid FormsFolderId{ get; set; }
    }
}