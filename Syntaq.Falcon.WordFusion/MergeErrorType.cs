using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace WordFusion.Assembly.MailMerge{
    public enum MergeErrorType{
        InvalidMergeField = 1,
        MissingRegionEnd = 2,
        RegionStartAndEndDifferentParents = 3,
        RegionEmpty = 4,
        InvalidRegionFilter = 5,
        InvalidListFormat = 6,
        InvalidRegion = 7
    }
}
