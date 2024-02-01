using System;

namespace Syntaq.Falcon.Tenants.Dashboard.Dto
{
    public class GetSubmissionLimitInput
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}