using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.Tenants.Dashboard.Dto
{
    public class GetTodaySubmissionsInput
    {
        public DateTime YesterdaysStartDate { get; set; }
        public DateTime YesterdaysEndDate { get; set; }
        public DateTime TodaysStartDate { get; set; }
        public DateTime TodaysEndDate { get; set; }
    }
}
