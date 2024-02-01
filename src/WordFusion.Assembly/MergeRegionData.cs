using System;
using System.Data;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Aspose.Words;

namespace WordFusion.Assembly.MailMerge
{
    public class MergeRegionData
    {
        public MergeRegionData(DataTable data)
        {
            this.Data = data;
            this.Name = data.TableName;
            this.IdColumn = null;
            this.ParentIdColumn = null;
        }

        public MergeRegionData(DataTable data, string name)
        {
            this.Data = data;
            this.Name = name;
            this.IdColumn = null;
            this.ParentIdColumn = null;
        }

        public MergeRegionData(DataTable data, string name, string idColumn)
        {
            this.Data = data;
            this.Name = name;
            this.IdColumn = idColumn;
            this.ParentIdColumn = null;
        }

        public MergeRegionData(DataTable data, string name, string idColumn, string parentIdColumn)
        {
            this.Data = data;
            this.Name = name;
            this.IdColumn = idColumn;
            this.ParentIdColumn = parentIdColumn;
        }

        public MergeRegionData(DataTable data, string name, string idColumn, string parentIdColumn, string sortBy)
        {
            this.Data = data;
            this.Name = name;
            this.IdColumn = idColumn;
            this.ParentIdColumn = parentIdColumn;
            this.SortBy = sortBy;
        }

        public string Name
        {
            get;
            set;
        }

        public string IdColumn
        {
            get;
            set;
        }

        public string ParentIdColumn
        {
            get;
            set;
        }

        public string SortBy
        {
            get;
            set;
        }

        public DataTable Data
        {
            get;
            set;
        }
    }
}
