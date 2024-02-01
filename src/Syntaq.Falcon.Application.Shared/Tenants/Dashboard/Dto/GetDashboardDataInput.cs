using System;

namespace Syntaq.Falcon.Tenants.Dashboard.Dto
{
	// MERGE REFACTOR
    public class GetDashboardDataInput
    {
		//public SalesSummaryDatePeriod SalesSummaryDatePeriod { get; set; }

		//STQ MODIFIED
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public DateTime YesterdaysStartDate { get; set; }
		public DateTime YesterdaysEndDate { get; set; }
		public DateTime TodaysStartDate { get; set; }
		public DateTime TodaysEndDate { get; set; }

	}
}