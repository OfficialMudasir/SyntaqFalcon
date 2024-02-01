using Newtonsoft.Json.Linq;
using Syntaq.Falcon.Forms.Dtos;
using System;
using System.Collections.Generic;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Forms
{
	public class FormSettingsViewModel
	{
		public Guid? FormId { get; set; }
		public string FormName { get; set; }
		public string FormVersionName { get; set; }
		public string FormDescription { get; set; }
		public bool IsEnabled { get; set; }
		public bool LockOnBuild { get; set; } = false;
        public bool LockToTenant { get; set; } = false;
        public bool RequireAuth { get; set; } = false;
        public bool IsPaymentProviderSet { get; set; }
		public bool IsPaymentEnabled { get; set; }
		public decimal PaymentAmount { get; set; }
		public string PaymentCurrency { get; set; }
		public string PaymentProcess { get; set; }

        public List<FormListDto> FormsList { get; set; }
    }
}