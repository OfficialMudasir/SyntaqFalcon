using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.Tenants.Dashboard.Dto
{
    public class GetDocumentsStatusCountInput
    {
        public char statusType { get; set; }
        public Guid? EnvironmentId { get; set; }
    }
}
