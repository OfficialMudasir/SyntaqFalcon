using System;
using System.IO;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Drawing;
using Aspose.Words;
using System.Data;

namespace WordFusion.Assembly.MailMerge
{
    public class MergeFieldEventArgs
    {
        public Document Document { get; internal set; }
        public MergeField Field { get; internal set; }
        public DataRow Row { get; internal set; }
        public string FieldName { get; internal set; }
        public string FieldValue { get; internal set; }
        public string TableName { get; internal set; }
        public string Text { get; set; }
    }
}