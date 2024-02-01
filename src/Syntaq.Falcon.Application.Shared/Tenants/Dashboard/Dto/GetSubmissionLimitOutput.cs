using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.Tenants.Dashboard.Dto
{
    public class GetSubmissionLimitOutput
    {
        public int SubmissionLimit { get; set; }
        public int CurrentSubmissions { get; set; }
        public int SubmissionUsagePercent { get; set; }
    }
}
