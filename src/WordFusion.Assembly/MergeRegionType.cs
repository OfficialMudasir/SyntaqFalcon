using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace WordFusion.Assembly.MailMerge
{
    public enum MergeRegionType{
        Table = 1,
        Conditional = 2,
        ListFormat = 3,
        None = 4
    }

    public enum MergeRegionOrder {
        TableAndConditional = 1,
        ListFormat=2
    }
}
