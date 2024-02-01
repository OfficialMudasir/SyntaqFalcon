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
    public class MergeImageFieldEventArgs
    {
        public Document Document { get; internal set; }
        public MergeField Field { get; internal set; }
        public DataRow Row { get; internal set; }
        public string FieldName { get; internal set; }
        public string FieldValue { get; internal set; }
        public string ImageFileName { get; set; }
        public Image Image { get; set; }
        public double WidthPoints { get; set; }
        public double HeightPoints { get; set; }
        public bool MaintainAspectRatio { get; set; }
    }
}