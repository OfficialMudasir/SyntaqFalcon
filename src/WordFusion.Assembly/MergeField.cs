using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Aspose.Words;
using Aspose.Words.Fields;
using System.Collections.ObjectModel;

namespace WordFusion.Assembly.MailMerge
{
    public class MergeField
    {
        public MergeField()
        {
            this.Nodes = new Collection<Node>();
            this.Switches = new List<MergeFieldSwitch>();
            this.Merged = false;
        }

        public bool Merged
        {
            get;
            set;
        }

        public MergeRegion Region{
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public Boolean isTableRegion {
            get{
                Boolean retval = false;
                if(Name.ToLower().Contains("table:"))retval = true;
                if (Name.ToLower().Contains("tablestart:")) retval = true;
                if (Name.ToLower().Contains("tableend:")) retval = true;
                return retval;
            }
        }

        public Boolean isConditionalRegion {
            get {
                Boolean retval = false;
                if (Name.ToLower().Contains("if:")) retval = true;
                if (Name.ToLower().Contains("ifstart:")) retval = true;
                if (Name.ToLower().Contains("ifend:")) retval = true;
                if (Name.ToLower().Contains("iftmp:")) retval = true;
               
                return retval;
            }
        }

        public Boolean isListFormatRegion
        {
            get
            {
                Boolean retval = false;
                if (Name.ToLower().Contains("listformatstart:")) retval = true;
                if (Name.ToLower().Contains("listformatend:")) retval = true;
                return retval;
            }
        }

        public Boolean isListItem
        {
            get
            {
                Boolean retval = false;
                if (Name.ToLower().Contains("listitem:")) retval = true;
                return retval;
            }
        }
        
        public FieldStart Start
        {
            get;
            set;
        }

        public FieldEnd End
        {
            get;
            set;
        }

        public Collection<Node> Nodes
        {
            get;
            set;
        }

        public IEnumerable<MergeFieldSwitch> Switches
        {
            get;
            set;
        }
    }
}
