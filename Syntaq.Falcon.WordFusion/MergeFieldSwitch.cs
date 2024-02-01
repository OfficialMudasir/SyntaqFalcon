using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Aspose.Words;
using Aspose.Words.Fields;

namespace WordFusion.Assembly.MailMerge
{
    public class MergeFieldSwitch
    {
        public MergeFieldSwitch(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }
    }
}
