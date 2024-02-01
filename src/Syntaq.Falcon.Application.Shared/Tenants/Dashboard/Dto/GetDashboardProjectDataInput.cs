using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.Tenants.Dashboard.Dto
{
    public class GetDashboardProjectDataInput
    {
        public char TabType { get; set; }
        public string ProjectTemplateId { get; set; }
        public Guid? EnvironmentId { get; set; }

    }
}
