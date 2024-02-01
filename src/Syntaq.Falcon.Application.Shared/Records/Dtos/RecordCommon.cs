using System;
using System.Collections.Generic;
using System.Text;
using static Syntaq.Falcon.Records.RecordMatterContributorConsts;

namespace Syntaq.Falcon.Records.Dtos
{
    public class ApplyInput
    {
        public string AccessToken { get; set; }
        public RecordMatterContributorStatus Status { get; set; }
    }

    public class UpdateCommentsInput
    {
        public string AccessToken { get; set; }
        public string Comments { get; set; }
    }
}
