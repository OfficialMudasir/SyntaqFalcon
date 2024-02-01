using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace WordFusion.Assembly.MailMerge{
    public class MergeError{
        public MergeError(MergeErrorType type, string node, string description){
            this.Type = type;
            this.Node = node;
            this.Description = description;
        }

        public MergeErrorType Type;
        public string Node;
        public string Description;
    }
}
