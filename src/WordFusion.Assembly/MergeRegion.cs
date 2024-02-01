using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Aspose.Words;

namespace WordFusion.Assembly.MailMerge
{
    [Serializable]
    public class MergeRegion
    {
        public MergeRegion()
        {
            this.Nodes = new List<Node>();
            this.ChildRegions = new List<MergeRegion>();
        }

        public MergeRegion ParentRegion {
            get;
            set;
        }

        public string Key { get; set; } // Used for the cache

        public string Name
        {
            get;
            set;
        }

        public System.Data.DataRow ParentDataRow {
            get;
            set;
        }

        public MergeRegionType Type
        {
            get;
            set;
        }

        private String _filter;
        public string Filter
        {
            get {
                return CleanFilter();
            }
            set{
                _filter = value;   
            }
        }

        private String CleanFilter( ) {
            String retval = _filter;

            if (! string.IsNullOrEmpty(retval)) {
                // Smart Quotes
                if (retval.IndexOf('\u2019') > -1) retval = retval.Replace('\u2019', '\'');
                if (retval.IndexOf('\u2018') > -1) retval = retval.Replace('\u2018', '\'');

                if (retval.IndexOf('\"') > -1) retval = retval.Replace('\"', '\'');

                retval = retval.ToLower().Replace("contains", "like");
            }

            return retval;
        }

        public Node Start
        {
            get;
            set;
        }

        public Node End
        {
            get;
            set;
        }

        public IEnumerable<Node> Nodes {
            get;
            set;
        }

        public IEnumerable<MergeRegion> ChildRegions
        {
            get;
            set;
        }


    }
}
