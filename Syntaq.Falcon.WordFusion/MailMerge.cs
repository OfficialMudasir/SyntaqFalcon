using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using Aspose.Words;
using Aspose.Words.Fields;
using Aspose.Words.Tables;
using Aspose.Words.Drawing;
using System.IO;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using Force.DeepCloner;
using System.Xml.Linq;
using System.Net;

namespace WordFusion.Assembly.MailMerge {
    /// <summary>

    public static class RegionCache
    {
        public static List<MergeRegion> Regions { get; set; } = new List<MergeRegion>() ;
    }

    public static class MailMergeCache
    {
        public static List<MailMergeCacheItem> MailMergeCacheItems { get; set; } =  new List <MailMergeCacheItem>();
    }

    public class MailMergeCacheItem
    {
        
        public string Key { get; set; }
        public DateTime DateUpdated { get; set; }
        public MailMerge MailMerge { get; set; }
    }

    public static class ObjectExtension
    {
        public static object CloneObject(object objSource)
        {

            var B = objSource.DeepClone();
            return B;   
        }
    }

    /// </summary>
    [Serializable]
    public class MailMerge {


        NodeList FieldStarts;

        public event InsertHandlerEvent InsertHandler;

        private List<MergeError> ErrorList;
        public List<MergeRegion> RegionList;
        public List<MergeRegion> ListFormatRegionList;
        public List<MergeRegionData> RegionData;

        public MailMerge() {
            MergeFieldFormatter.InsertHandler += MergeFieldFormatter_InsertHandler;
            MergeFieldFormatter.document = Document;
            InitData();
        }

        private void MergeFieldFormatter_InsertHandler(Document document, MergeField mergeField, string switchValue) {
            InsertHandler(this.Document, mergeField, switchValue);
        }

        public void InitData() {
            // Initialise region data
            this.RegionList = new List<MergeRegion>();
            this.ListFormatRegionList = new List<MergeRegion>();
            this.RegionData = new List<MergeRegionData>();
        }

        /// <summary>
        /// Loads an Aspose.Words Document object for merging.
        /// The document is validated when loaded, and the Errors property is populated with any 
        /// errors found in the document.
        /// The Regions property is also populated with any regions found in the document.
        /// </summary>
        /// 
        /// <param name="document">An Aspose.Words Document object.</param>
        public void Load(Document document) {


            this.Document = document;
            FieldStarts = this.Document.SelectNodes("//FieldStart");
            // Initialise error list
            this.ErrorList = new List<MergeError>();

            this.GetRegionsForLevel(this.Document.Document, this.RegionList, null, MergeRegionOrder.TableAndConditional);

        }

        public void AddRegionData(DataTable data) {
            this.RegionData.Add(new MergeRegionData(data));
        }

        public void AddRegionData(DataTable data, string name) {
            this.RegionData.Add(new MergeRegionData(data, name));
        }

        public void AddRegionData(DataTable data, string name, string idColumn) {
            this.RegionData.Add(new MergeRegionData(data, name, idColumn));
        }

        public void AddRegionData(DataTable data, string name, string idColumn, string parentIdColumn) {
            this.RegionData.Add(new MergeRegionData(data, name, idColumn, parentIdColumn));
        }

        public void AddRegionData(DataTable data, string name, string idColumn, string parentIdColumn, 
            string sortBy) {

            this.RegionData.Add(new MergeRegionData(data, name, idColumn, parentIdColumn, sortBy));
        }

        public void ClearAllData() {
            this.DocumentData = null;
            this.RegionData.Clear();
        }

        /// <summary>
        /// Merges the document provided in the Load method with the data provided in the 
        /// AddRegionData methods.
        /// Note that if the document contains errors, Merge will throw an exception. You should 
        /// always check the Errors property before calling this method.
        /// </summary>
        public void Merge() {

            if (DocumentData.Table.Columns.Contains("Assembly_Debug")) {
                bool.TryParse(DocumentData["Assembly_Debug"].ToString(), out debug);
            }

            // If there are any errors then we cannot perform a merge
            if (this.ErrorList.Count > 0) {
                throw (new ApplicationException("Document contains errors. Always check the " +
                    "Errors property before calling Merge()."));
            }

            // Merge each region
            Node insertAfter = null;

            RunRuleFields();

            // Merge the Table and Conditional Regions First
            foreach (MergeRegion region in this.Regions) {

                if(region.End != null) {
                    insertAfter = region.End;
                    this.MergeRegion(region, null, insertAfter.ParentNode, insertAfter, null, null);
                    // Once the region is merged, it can be removed from the tree   
                }
                else {
                    this.ErrorList.Add(new MergeError(MergeErrorType.RegionStartAndEndDifferentParents, "Rule", "Error in Region " + region.Name + " field."));
                }

                this.RemoveRegion(region);
               
            }
            
            // Load ListFormat Regions
            // Merge ListFormat Regions           
            this.RegionList = new List<MergeRegion>();
            insertAfter = null;
            this.GetRegionsForLevel(this.Document.Document, this.RegionList, null, MergeRegionOrder.ListFormat);

            foreach (MergeRegion region in this.Regions) {
                insertAfter = region.End;
                this.MergeRegion(region, null, insertAfter.ParentNode, insertAfter, null, null);
                // Once the region is merged, it can be removed from the tree                            
            }

            // Once all regions are merged, merged any document-level data
            this.MergeDocumentData();

            // Remove any empty paragraphs
            NodeList fields = this.Document.SelectNodes("//Paragraph[Run='REMOVE_PARA']");
            foreach (Node field in fields) {
                field.Remove();
            }

        }

        // Run rules based on Rule Switch
        //MERGEFIELD  Rule:  \Work_yn* = "No"  \DelPara  \* MERGEFORMAT
        private void RunRuleFields(){

            string currentrule = string.Empty;

            try {

                if(Document.MailMerge != null) {

                }

                string[] str = Document.MailMerge.GetFieldNames();

                // Run any rules
                NodeList rules = this.Document.SelectNodes("//FieldStart");
                DocumentBuilder docbuilder = new DocumentBuilder(Document);

                foreach (Node node in rules) {
                    MergeField mergeField = this.GetMergeField((FieldStart)node);
 
                    if (mergeField.Name != null) {
                        if (mergeField.Name.ToLower() == "rule:") {
                            
                            string fldname = mergeField.Switches.ElementAtOrDefault(0) != null ? mergeField.Switches.ElementAt(0).Name : string.Empty;
                            string fldval = mergeField.Switches.ElementAtOrDefault(0) != null ? mergeField.Switches.ElementAt(0).Value : string.Empty;
                            string action = mergeField.Switches.ElementAtOrDefault(1) != null ? mergeField.Switches.ElementAt(1).Name : string.Empty;

                            // Get the merge fields this applies to.
                            string[] flds = fldname.Contains("*") ?
                                Document.MailMerge.GetFieldNames().Where(i => i.StartsWith(fldname.Replace("*", ""))).ToArray() :
                                Document.MailMerge.GetFieldNames().Where(i => i == fldname).ToArray();

                            foreach (string fld in flds) {
                                // get the actual field name to check the value
                                if (DocumentData.Table.Columns.Contains(fld)) {

                                    string select = fld + " " + fldval.Replace("\"", "'");
                                    DataRow[] result = DocumentData.Table.Select(select  );

                                    if (result.Count() > 0) {

                                        bool ispara = docbuilder.MoveToMergeField(fld);

                                        if (ispara) { 

                                            switch (action.ToLower()) {
                                                case "delpara":
                                                    docbuilder.CurrentParagraph.Remove();
                                                    break;
                                                case "deltablerow":

                                                    Row row = (Row)docbuilder.CurrentNode.GetAncestor(NodeType.Row);
                                                    row.Remove();

                                                    break;
                                            }
                                        }

                                    }
                                }
                            }
 
                        }
                    }
                }

            }
            catch  {
                this.ErrorList.Add(new MergeError(MergeErrorType.InvalidMergeField, "Rule", "Check rule " + currentrule + " field."));
            }
        }

        public IEnumerable<MergeError> Errors {
            get {
                return ErrorList;
            }
        }

        public IEnumerable<MergeRegion> Regions {
            get {
                return RegionList;
            }
        }

        public IEnumerable<MergeRegion> ListFormatRegions {
            get {
                return ListFormatRegionList;
            }
        }

        public Document Document {
            get;
            set;
        }

        public DataRow DocumentData {
            private get;
            set;
        }

        /// <summary>
        /// All data fro this mail merge
        /// </summary>
        public DataSet Data{
            get;
            set;
        }

        public event MergeFieldEventHandler MergeField;
        public event MergeImageFieldEventHandler MergeImageField;

        private Node MergeRegion(MergeRegion region, CompositeNode sourceParent, CompositeNode destParent, Node insertAfter, DataRow parentDataRow, DataRow dataRow) {

            try { 
                switch (region.Type) {
                    case MergeRegionType.Table:
                        insertAfter = this.MergeTableRegion(region, sourceParent, destParent, insertAfter, parentDataRow);
                        break;
                    case MergeRegionType.Conditional:
                        insertAfter = this.MergeConditionalRegion(region, sourceParent, destParent, insertAfter, parentDataRow, dataRow);
                        break;
                    case MergeRegionType.ListFormat:
                        this.MergeListFormatRegion(region);
                        break;
                }

            }
            catch (Exception ex) {
                this.ErrorList.Add(new MergeError(MergeErrorType.InvalidRegion, region.Name + " has an error ", ex.Message));
            }

            return insertAfter;

        }

        private Node MergeTableRegion(MergeRegion region, CompositeNode sourceParent, CompositeNode destParent, Node insertAfter, DataRow parentDataRow) {

            // Do we have data for this region?
            MergeRegionData regionData = this.GetRegionData(region.Name, parentDataRow );

            if (regionData != null) {

                DataRow[] dataRows;
                string selectStatement = "";

                // Does this region have a filter?
                if (region.Filter != null) {
                    selectStatement = region.Filter;
                }

                if (selectStatement.ToLower().Contains("parent.parentid"))
                {
 
                    // Look for the parent region
                    int parentDataRowId = -1;
                    if (!Convert.IsDBNull(parentDataRow[regionData.ParentIdColumn]))
                    {
                        parentDataRowId = Convert.ToInt16(parentDataRow[regionData.ParentIdColumn]);
                    }
                    selectStatement = $"ParentID='{parentDataRowId}'";                  
                }
                else if (selectStatement.Trim().ToLower() != "*" && selectStatement.Trim().ToLower() != "all" && parentDataRow != null && regionData.ParentIdColumn != null) {
                    if (selectStatement != "") {
                        selectStatement += " AND ";
                    }

                    int parentDataRowId = -1;

                    if (!Convert.IsDBNull(parentDataRow[regionData.ParentIdColumn])) {
                        parentDataRowId = Convert.ToInt16(parentDataRow[regionData.IdColumn]);
                    }

                    selectStatement += regionData.ParentIdColumn + " = '" + parentDataRowId + "'";

                    //Get the Parent Table Name
                    if (parentDataRow != null) {
                        string tblnameparent = parentDataRow.Table.TableName;
                        string tblnameparentIdcol = tblnameparent + "_Id";
                        if (parentDataRow.Table.Columns.Contains(tblnameparentIdcol)) {
                            object tblparentid = parentDataRow[tblnameparentIdcol];

                            selectStatement += " and ( "  + tblnameparentIdcol + " = " + tblparentid.ToString() + ")";

                        }
                    }

                }

                if (regionData.SortBy != null) {
                    dataRows = regionData.Data.Select(selectStatement, regionData.SortBy);
                }
                else {
                    try {
                        if (selectStatement.Trim().ToLower() == "all" || selectStatement.Trim().ToLower() == "*") {
                            dataRows = regionData.Data.Select("");
                        }
                        else{
                            dataRows = regionData.Data.Select(selectStatement);
                        }                        
                    }
                    catch {
                        dataRows = regionData.Data.Select("");
                    }                    
                }

                // Iterate through each data row and merge
                foreach (DataRow dataRow in dataRows) {
                    insertAfter = this.InsertRegionNodes(region, dataRow, sourceParent, destParent, insertAfter, dataRow);
                }
            }

            return insertAfter;
        }

        private MergeRegionData GetRegionData(string regionName, DataRow parentDataRow) {

            MergeRegionData retval = null;

            if (regionName.StartsWith("SS[", StringComparison.CurrentCultureIgnoreCase)) {

            }
            else {
                foreach (MergeRegionData region in RegionData) {
                    if (region.Name.ToLower() == regionName.ToLower()) {
                        retval = region;
                        break;
                    }
                }
            }

            if (retval == null && regionName == "File") {
                retval = GetFileRegionData();
            }

            return retval;
        }

        private MergeRegionData GetFileRegionData() {

            MergeRegionData retval = null;

            try {
                DataTable dt = DocumentData.Table;
                retval = new MergeRegionData(dt)
                {
                    Name = "File",
                    ParentIdColumn = "ParentID",
                    IdColumn = "ID"
                };
            }
            catch (Exception ex) {
                throw ex;
            }
            finally {
 
            }

            return retval;
        }

        bool debug = false;

        // Log if Neccessary
        [System.Diagnostics.DebuggerStepThrough]
        private Node MergeConditionalRegion(MergeRegion region, CompositeNode sourceParent, CompositeNode destParent, Node insertAfter, DataRow parentDataRow, DataRow dataRow) {

            // Construct a DataTable that combines the region data with global data
            DataTable data = this.GetConditionalDataTable(dataRow);

            // Now that we've constructed the DataTable, we can perform a select query
            DataRow[] result = null;
            MergeRegion parenttableregion = FindParentTableRegion(region.ParentRegion);
            String name = region.Name;

            if (String.IsNullOrEmpty(region.Filter)) region.Filter = "1 = 1";

            // Filters true or false on a check from the parent data
            string filter = ParentRegionDataCheck(region, parentDataRow);

            String colname = filter.ToString().Split("=".ToCharArray()).GetValue(0).ToString().Trim();

            if (parenttableregion == null && region.Type == MergeRegionType.Conditional) {

                // If a column is not there then the result is treated as false
                try {

                    // if this is a count filter then add the column and to 0 if not exists
                    if (filter.ToLower().Contains("_count")) {                        
                        try {
                            result = data.Select(filter);
                        }
                        catch (EvaluateException)
                        {
                            try {
                                MergeConditionalRegion_AddCountField(ref data, filter);                           
                            }
                            catch {
                                result = data.Select(filter);
                            }
                        }
                    }

                    result = data.Select(filter);
                    
                }
                catch (SyntaxErrorException)
                {
                    this.ErrorList.Add(new MergeError(MergeErrorType.InvalidRegionFilter, region.Name,  "The region " + region.Name + " " + filter + " Filter is invalid. "));
                    
                }
                catch { // Filter = false 

                }
            }
            else {

                // If a column is not there then the result is treated as false
                try {
                    // if this is a count filter then add the column and set to 0 if not exists
                    if (filter.ToLower().Contains("_count")) {
                        try {
                            result = data.Select(filter);
                        }
                        catch (EvaluateException)
                        {
                            try {
                                MergeConditionalRegion_AddCountField(ref data, filter);
                            }
                            catch {
                                result = data.Select(filter);
                            }
                        }
                    }

                    if (filter.Trim().ToLower().Contains("file."))
                    {
                        var filefliter = filter;
                        var filematches = Regex.Matches(filefliter, @"file.([^\ ]+)", RegexOptions.IgnoreCase);
                        foreach (Match filematch in filematches)
                        {
                            var filecol = Regex.Replace(filematch.Value, "file.", "", RegexOptions.IgnoreCase);
                            filecol = filecol.ToString().Replace("'", "");

                            var filecolvalue = DocumentData.Table.Rows[0][filecol];

                            // Examples
                            // gift_resid_tt_benef_dies_cho = 'testator' and file.tt_type_cho = 'ptt'
                            // gift_resid_tt_benef_dies_cho = 'testator' and 'file.tt_type_cho' = 'ptt'

                            if (filematch.Value.EndsWith("'"))
                            {
                                filefliter = Regex.Replace(filefliter, "file." + filecol, filecolvalue.ToString(), RegexOptions.IgnoreCase);
                            }
                            else
                            {
                                filefliter = Regex.Replace(filefliter, "file." + filecol,  "'" + filecolvalue.ToString() + "'", RegexOptions.IgnoreCase);
                            }
                        }

                        result = data.Select(filefliter);

                    }
                    else
                    {
                        result = data.Select(filter);
                    }
                    
                }
                catch (SyntaxErrorException)
                {
                    ErrorList.Add(new MergeError(MergeErrorType.InvalidRegionFilter, region.Name, "The region " + region.Name + " " + filter + " Filter is invalid. "));
                }
                catch { // Filter = false 
                }

            }

            if (result != null) {
                if (result.Length > 0 || debug) {
                    // Condition is true, insert a copy of the region nodes
                    if(result.Length > 0) dataRow = (DataRow)result.GetValue(0); // .First();
      
                    insertAfter = this.InsertRegionNodes(region, dataRow, sourceParent, destParent, insertAfter, parentDataRow);        
                }
                else {
                    // Condition is false, do nothing     
                }
            }
            //
            return insertAfter;
        }

        private void Settextcolor(Aspose.Words.CompositeNode parentnode, Color color) {
            foreach (Node node in parentnode) {
                if (node is CompositeNode) {
                    Settextcolor((CompositeNode)node, color);
                }
                else if (node is Run runnode)
                {
                    runnode.Font.Color = color;
                }
            }
        }

        private string FileRegionDataCheck(string filter)
        {
            string retval = filter;

            var rows = DocumentData.Table.Select(filter);

            if(rows.Count() > 0)
            {
                retval = " (1==1) ";
            }

            return retval;
        }

        private string ParentRegionDataCheck(MergeRegion region, DataRow parentrow) {

            string retval = region.Filter;

            if (parentrow != null){
            
                DataSet dataset = parentrow.Table.DataSet;
                DataTable datatable = parentrow.Table;

                try {

                    if (region.ParentRegion != null && parentrow != null){
                        retval = ParseRegionOrFieldValue(region.Filter, parentrow);
                    }
                }
                catch(Exception ex) {
                    throw (new Exception ( ex.Message + ": " + region.Name + ": " + region.Filter) );
                }
            }

            return retval;

        }

        private string ParseRegionOrFieldValue(string input, DataRow datarow){

            var retval = input;

            DataSet dataset = datarow.Table.DataSet;
            DataTable datatable = datarow.Table;

            var a = input.Split('\'', '.', ' ');

            int offset = a.Length < 3 ? 1 : 0;

            for (int i = 0; i < a.Count() - 2 + offset; i++){

                var c = a[i + 1 ]; // next
                var d = a[i + 2 - offset]; // tablename or field (next.gift_level_rpt.choice_cho or next.choice_cho)

                if (a[i].ToLower() == "next")
                {

                    if (dataset.Tables.Contains(c))
                    { // then the next is for a table

                        DataTable e = dataset.Tables[c];
                        var f = datarow.GetParentRow(datatable.ParentRelations[0]);

                        int id = Convert.ToInt16(f["rpt_Index"]) + 1;
                        int parentid = Convert.ToInt16(f["ParentID"]);
                        var g = e.Select("ParentID = '" + parentid + "' and rpt_Index='" + id + "'");

                        if (g.Count() > 0)
                        {
                            if (e.Columns.Contains(d))
                            {
                                var j = g[0][d].ToString(); // fieldvalue
                                retval = Regex.Replace(retval, "next." + c + "." + d, j, RegexOptions.IgnoreCase);
                            }
                        }
                        else
                        {
                            retval = Regex.Replace(retval, "next." + c + "." + d, Guid.NewGuid().ToString() , RegexOptions.IgnoreCase);
                        }

                        i += 3; // skip 3 next.gift_level_rpt.choice_cho
                    }
                    else if (datatable.Columns.Contains(c))
                    { // the next is for the filed in the next row

                        int id = Convert.ToInt16(datarow["rpt_Index"]) + 1;
                        int parentid = Convert.ToInt16(datarow["ParentID"]);
                        var nextrows = datarow.Table.Select("ParentID = '" + parentid + "' and rpt_Index='" + id + "'");

                        if (nextrows.Count() > 0){
                            var nextrow = nextrows[0];
                            var e = Convert.ToString(nextrow[c]);
                            retval = Regex.Replace(retval, "next." + c, e, RegexOptions.IgnoreCase);
                            i += 2;
                        }
                        else{
                            retval = Regex.Replace(retval, "next." + c + "." + d, Guid.NewGuid().ToString(), RegexOptions.IgnoreCase);
                        }
                    }
                }
                else if (dataset.Tables.Contains(c)){

                    // then the next is for a table

                    DataTable e = dataset.Tables[c];
                    int parentid = Convert.ToInt16(datarow["ParentID"]);
                    var g = e.Select("ID = '" + parentid + "'");

                    if (g.Count() > 0)
                    {
                        if (e.Columns.Contains(d))
                        {
                            var j = g[0][d].ToString(); // fieldvalue
                            retval = Regex.Replace(retval, c + "." + d, j, RegexOptions.IgnoreCase);
                        }
                    }
                    i += 2; // skip 3 next.gift_level_rpt.choice_cho                    
                }
            }

            return retval;

        }

        private void MergeConditionalRegion_AddCountField(ref DataTable data, String filter) {

            // if this is a count filter then add the column and set to 0 if not exists
            char[] delimiters = new char[] { '=', '<', '>' };
            String[] filters = filter.Trim().Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            DataColumn col = new DataColumn(filters[0].Trim())
            {
                DataType = Type.GetType("System.String")
            };
            data.Columns.Add(col);

            foreach (DataRow row in data.Rows) {
                row[col] = "0";
            }
            data.AcceptChanges();
 
        }


        private DataTable GetConditionalDataTable(DataRow dataRow) {

            DataTable data = new DataTable();
            DataRow row = null;

            // Add region data if it exists
            if (dataRow != null) {
                data = dataRow.Table.Clone();
                data.ImportRow(dataRow);

                // If the original data row was not added to the data table (e.g. in ValidateRegionFilter)
                // then ImportRow will not do anything.
                if (data.Rows.Count > 0) {
                    row = data.Rows[0];
                }
            }

            // Add global data if it exists
            if (this.DocumentData != null) {
                // Add document data columns
                foreach (DataColumn column in this.DocumentData.Table.Columns) {
                    if (!data.Columns.Contains(column.ColumnName)) {
                        data.Columns.Add(new DataColumn(column.ColumnName, column.DataType));
                    }
                }

                if (row == null) {
                    row = data.NewRow();
                    row = PopulateDummyRow(row);
                    data.Rows.Add(row);
                }

                // Populate document data
                foreach (DataColumn column in this.DocumentData.Table.Columns) {
                    if (data.Rows[0][column.ColumnName] == DBNull.Value) {
                        data.Rows[0][column.ColumnName] = this.DocumentData[column.ColumnName];
                    }
                }
            }

            return data;
        }

        private void MergeListFormatRegion(MergeRegion region) {

            // Get The count of Items            
            int rowcnt = 0;
            MergeListFormatItemsRowCnt(region, ref rowcnt);

            int rowindex = 0;

            // Construct a DataTable that combines the region data with global data
            foreach (Node node in region.Nodes) {

                DocumentBuilder builder = new DocumentBuilder(Document);
                if (node.GetType() == typeof(FieldStart)) {

                    MergeField mergeField = this.GetMergeField((FieldStart)node);

                    if (mergeField.Start != null) {
                        if (mergeField.Name.ToLower() == "listitem:" + region.Name.ToLower()) {
                            String lstValue = MergeFieldListFormat.FormatListValue(region.Filter, rowindex, rowcnt);
                            builder.MoveTo(node);
                            builder.Write(lstValue);

                            RemoveMergeField(mergeField);

                            rowindex++;
                        }
                    }
                }

                if (node is CompositeNode compnode)
                {
                    MergeListFormatItems(compnode, region.Name, region.Filter, ref rowindex, rowcnt);
                }
            }


            // Merge Child List Formats
            foreach (MergeRegion childregion in region.ChildRegions) {
                MergeListFormatRegion(childregion);
            }

        }

        private void MergeListFormatItems(CompositeNode parentnode, String regionname, String listname, ref int rowindex, int rowcnt) {

            try
            {

                DocumentBuilder builder = new DocumentBuilder(Document);

                foreach (Node node in parentnode.ChildNodes)
                {

                    if (node.GetType() == typeof(FieldStart))
                    {
                        MergeField mergeField = this.GetMergeField((FieldStart)node);

                        if (IsMergeField((FieldStart)node))
                        {
                            if (mergeField.Name.ToLower() == "listitem:" + regionname.ToLower())
                            {

                                String lstValue = MergeFieldListFormat.FormatListValue(listname, rowindex, rowcnt);
                                builder.MoveTo(node);
                                builder.Write(lstValue);

                                rowindex++;
                            }
                        }

                    }

                    if (node is CompositeNode compnode)
                    {
                        MergeListFormatItems(compnode, regionname, listname, ref rowindex, rowcnt);
                    }
                }

            }
            catch 
            {
                this.ErrorList.Add(new MergeError(
                    MergeErrorType.InvalidMergeField,
                    regionname,
                    "The region " + regionname + " has an error. Check that the List format type ahs been set"));
            }

        }

        private void MergeListFormatItemsRowCnt(MergeRegion region, ref int rowcnt) {

            try {

                foreach (Node node in region.Nodes) {
                    if (node.GetType() == typeof(FieldStart)) {
                        MergeField mergeField = this.GetMergeField((FieldStart)node);

                        if (mergeField.Start != null) {
                            if (mergeField.Name.ToLower() == "listitem:" + region.Name.ToLower()) {
                                rowcnt++;
                            }
                        }

                    }
                    if (node is CompositeNode compnode)
                    {
                        MergeListFormatItemsRowCnt(compnode, region.Name, ref rowcnt);
                    }
                }
            }
            catch (Exception ex) {
                throw ex;
            }


        }

        private void MergeListFormatItemsRowCnt(CompositeNode parentnode, String regionname, ref int rowcnt) {

            try {

                foreach (Node node in parentnode.ChildNodes) {
                    if (node.GetType() == typeof(FieldStart)) {

                        if (IsMergeField((FieldStart)node)) {
                            MergeField mergeField = this.GetMergeField((FieldStart)node);
                            if (mergeField.Name.ToLower() == "listitem:" + regionname.ToLower()) {
                                rowcnt++;
                            }
                        }

                    }
                    if (node is CompositeNode compnode)
                    {
                        MergeListFormatItemsRowCnt(compnode, regionname, ref rowcnt);
                    }
                }

            }
            catch (Exception)
            {
                throw new Exception("Error in ListFormat:" + regionname + "; row " + rowcnt);
            }


        }

        private DataRow PopulateDummyRow(DataRow row) {

            foreach (DataColumn column in row.Table.Columns) {
                if (column.AllowDBNull == false) {
                    switch (column.DataType.ToString()) {
                        case "System.String":
                            row[column.ColumnName] = "";
                            break;
                        case "System.DateTime":
                            row[column.ColumnName] = new DateTime();
                            break;
                        case "System.DateTimeOffset":
                            row[column.ColumnName] = new DateTimeOffset();
                            break;
                        case "System.Boolean":
                            row[column.ColumnName] = false;
                            break;
                        default:
                            row[column.ColumnName] = 0;
                            break;
                    }
                }
            }

            return row;
        }

        private void MergeDocumentData() {

            // Get all remaining merge fields
            NodeList fields = this.Document.SelectNodes("//FieldStart");
            MergeField mergeField;

            if (this.DocumentData != null) {
                foreach (Node field in fields) {
                    mergeField = this.GetMergeField((FieldStart)field);
                    if (mergeField != null) {
                        this.SetMergeField(mergeField, this.DocumentData);
                    }
                }
            }
        }

        private Node InsertRegionNodes(MergeRegion region, DataRow dataRow, CompositeNode sourceParent, CompositeNode destParent, Node insertAfter, DataRow parentDataRow) {

            MergeRegion subRegion;
            Node newNode;
            Node skipUntilNode = null;
            IEnumerable<Node> sourceNodes;
            List<Node> insertedNodes = new List<Node>();

            try {

                if (sourceParent == null) {
                    sourceNodes = region.Nodes;
                }
                else {
                    sourceNodes = this.GetChildNodes(sourceParent);
                }

                int i = 0;

                Aspose.Words.Lists.List newlist = null; // Needto restart the numbering of numbered lists in cloned sections
                Node clonednode = null; 
                foreach (Node node in sourceNodes) {

                    //// Need to restart the numbering of numbered lists in cloned sections
                    //if (node.NodeType.Equals(NodeType.Paragraph))
                    //{
                    //    var nodepara = (Paragraph)node;
                    //    if (nodepara.IsListItem)
                    //    {                            
                    //        if (newlist != nodepara.ListFormat.List) {
                    //            newlist = Document.Lists.AddCopy(nodepara.ListFormat.List);
                    //        }

                    //        while (nodepara != null && nodepara.IsListItem)
                    //        {
                    //            nodepara.ListFormat.List = newlist;

                    //            if (nodepara.NextSibling.NodeType.Equals(NodeType.Paragraph))
                    //            {
                    //                nodepara = (Paragraph)nodepara.NextSibling;
                    //            }
                    //            else
                    //            {
                    //                nodepara = null; // ie. table
                    //            }

                    //        }
                    //    }
                    //}

                    i++;
                    // Is this node the start of another region?
                    subRegion = this.GetRegionFromStartNode(region.ChildRegions, node, region.Type);
                   
                    if (subRegion != null) {
                        // Yes, so merge the sub region
                        insertAfter = this.MergeRegion(subRegion, null, destParent, insertAfter, parentDataRow, dataRow);

                        // Tell the current loop to skip all nodes until the end of this region
                        if (subRegion.Start != subRegion.End) {
                            skipUntilNode = subRegion.End;
                        }

                    }
                    else {
                        if (skipUntilNode == null) {

                            // Create a copy of the node to add to the document
                            newNode = node.Clone(false);

                            //Node[] paragraphs = newNode.GetChildNodes(NodeType.li, true).ToArray();
                            //NodeList resultList = newNode.SelectNodes("//FieldStart/following-sibling::node()[following-sibling::FieldEnd]");

                            //CompositeNode cn = (CompositeNode)newNode;
                            //cn.SelectNodes("//FieldStart/following-sibling::node()[following-sibling::FieldEnd]");

                            clonednode = node;
                            
                            // Insert the node into the tree
                            if (insertAfter == null) {
                                destParent.PrependChild(newNode);
                            }
                            else {
                                if (insertAfter.ParentNode != null) {
                                    insertAfter.ParentNode.InsertAfter(newNode, insertAfter);
                                }
                            }

                            // Does this node have children?
                            if (node.IsComposite) {
                                // Yes, so recurse
                                this.InsertRegionNodes(region, dataRow, (CompositeNode)node, (CompositeNode)newNode, null, parentDataRow);
                            }
                  
                            insertedNodes.Add(newNode);
                            insertAfter = newNode;

                        }

                        // If the current node is the skip-until-node, then clear it
                        if (skipUntilNode == node) {
                            skipUntilNode = null;
                        }
                    }
                }

                // Once all nodes are added, set any merge fields
                if (this.FindParentTableRegion(region) != null) {
                    this.MergeNodes(insertedNodes, dataRow);
                }

                if (insertAfter != null) {
                    if (insertAfter.ParentNode == null) {
                        insertAfter = clonednode;
                    }
                }

            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }

            return insertAfter;
        }

        private void GetRegionsForLevel(CompositeNode parentNode, List<MergeRegion> regionList, MergeRegion parentRegion, MergeRegionOrder regionorder) {

            // A region is defined by 2 merge fields with the syntax TableStart:MyTable and 
            int i = 0;
            for (i = 0; i < parentNode.ChildNodes.Count + 1; i++) {

                Node node = parentNode.ChildNodes[i];

                if (node != null) {
                    if (node.GetType() == typeof(FieldStart)) {
                        this.AddRegion((FieldStart)node, regionList, parentRegion, regionorder);
                    }

                    if (node.IsComposite) {
                        // This is not a field, so recurse to look for child fields
                        this.GetRegionsForLevel((CompositeNode)node, regionList, parentRegion, regionorder);
                    }
                }
            }
        }


        private readonly Collection<string> RegionsList = new Collection<String>();
        private readonly Collection<FieldStart> RegionStarts = new Collection<FieldStart>();

        private void AddRegion(FieldStart fieldStart, List<MergeRegion> regionList, MergeRegion parentRegion, MergeRegionOrder regionorder) {

            MergeField regionStart = this.GetMergeField(fieldStart);
            MergeRegion region = this.GetRegion(regionStart, parentRegion, regionorder);

            // Is this field a region?
            if (region != null) {
                // Add the region to the object list
                regionList.Add(region);

                // This region may contain child regions, so loop through each node
                foreach (Node node in region.Nodes) {
                    if (node != null) {
                        if (node.GetType() == typeof(FieldStart)) {
                            AddRegion((FieldStart)node, (List<MergeRegion>)region.ChildRegions, region,  regionorder);
                        }
                        else if (node.IsComposite) {
                            // Not a region, but may contain children that are regions                         
                            this.GetRegionsForLevel((CompositeNode)node, (List<MergeRegion>)region.ChildRegions, region, regionorder);
                        }
                    }
                }
            }
        }

        public MergeRegion GetRegion(MergeField regionStart, MergeRegion parentRegion, MergeRegionOrder regionorder) {

            MergeRegion region = null;

            if (regionStart.Name != null) {
                if (regionStart.Name.ToLower().Contains("tablestart:")) {
                    region = new MergeRegion
                    {
                        Type = MergeRegionType.Table
                    };
                }
                else if (regionStart.Name.ToLower().Contains("listformatstart:")) {
                    region = new MergeRegion
                    {
                        Type = MergeRegionType.ListFormat,
                        ParentRegion = parentRegion
                    };
                }
                else if (regionStart.Name.ToLower().Contains("ifstart")) {
                    region = new MergeRegion
                    {
                        Type = MergeRegionType.Conditional,
                        ParentRegion = parentRegion
                    };
                }
            }

            if (region != null) {
                if ((region.Type == MergeRegionType.Conditional || region.Type == MergeRegionType.Table) && regionorder == MergeRegionOrder.ListFormat) {
                    region = null;
                }
                else {
                    if (region.Type == MergeRegionType.ListFormat && regionorder == MergeRegionOrder.TableAndConditional) region = null;
                }
            }


            if (region != null) {

                ParseRegionValue(regionStart.Name, out string regionName, out string regionFilter);

                region.Name = regionName;
                region.Filter = regionFilter;

                if (parentRegion != null) region.ParentRegion = parentRegion;
                region.Filter = regionFilter;

                // Ensure the region filter is valid
                if (ValidateRegionFilter(region, parentRegion)) {
                    // Get the associated TableEnd merge field
                    string regionEndName;

                    if (region.Type == MergeRegionType.Table) {
                        regionEndName = "TableEnd";
                    }
                    else if (region.Type == MergeRegionType.ListFormat) {
                        regionEndName = "ListFormatEnd";
                    }
                    else {
                        regionEndName = "IfEnd";
                    }

                    MergeField regionEnd;
                    if (region.Type == MergeRegionType.Conditional) {
                        // 
                        // regionEnd = this.FindIFRegionEnd(regionStart);
                        regionEnd = this.FindMergeFieldByText(regionEndName + ":" + region.Name);
                    }
                    else {
                        regionEnd = this.FindMergeFieldByText(regionEndName + ":" + region.Name);
                    }                   

                    if (regionEnd != null) {

                        // Get the nodes contained in this region
                        List<Node> regionNodes = this.GetRegionNodes(region, regionStart, regionEnd);

                        if (regionNodes.Count > 0) {
                            region.Start = regionNodes[0];
                            region.End = regionNodes[regionNodes.Count - 1];
                            region.Nodes = regionNodes;
                        }
                        else {
                            this.ErrorList.Add(new MergeError(
                                MergeErrorType.RegionEmpty,
                                region.Name,
                                "The region " + region.Name + " is empty."));

                            region = null;
                        }
                    }
                    else {

                        DocumentBuilder builder = new DocumentBuilder(Document);
                        builder.MoveTo(regionStart.Start);

                        Aspose.Words.Font font = builder.Font;
                        font.Bold = true;
                        font.Color = Color.Red;
                        font.Italic = true;
                        font.Name = "Arial";

                        // Output formatted text
                        builder.Writeln("ERROR: Can't find the end of " + region.Name);

                        //foreach (Run run in builder.CurrentNode.)
                        //    run.Font.HighlightColor = Color.Yellow;

                        this.ErrorList.Add(new MergeError(
                            MergeErrorType.MissingRegionEnd,
                            region.Name,
                            "Missing " + regionEndName + " merge field for region " + region.Name + "."));

                        region = null;

                        string fldname = regionStart.Name;
                        int index = fldname.IndexOf(" ");
                        if (index > 0) fldname = fldname.Substring(0, index); // or index + 1 to keep slash

                        regionStart.Start.Remove();

                        builder.MoveToMergeField(fldname, false, true);
                    }
                }
                else {
                  
                    region = null;
                }
            }

            return region;
        }

        public List<Node> GetRegionNodes(MergeRegion region, MergeField regionStart, MergeField regionEnd) {

            List<Node> nodes = new List<Node>();
            Cell startParentCell = this.GetNodeParentCell(regionStart.Start);
            Cell endParentCell = this.GetNodeParentCell(regionEnd.End);

            // Is the region start or end within a table cell, and are they in different cells?
            if ((startParentCell != null && endParentCell != null) && (startParentCell != endParentCell)) {
                // Yes, do both rows belong to the same table?
                if (startParentCell.ParentNode.ParentNode != startParentCell.ParentNode.ParentNode) {
                    // No, so return an error
                    this.ErrorList.Add(new MergeError(
                        MergeErrorType.RegionStartAndEndDifferentParents,
                        region.Name,
                        "The start & end merge fields for the region " + region.Name + " are " + "in different tables."));
                }
                else {
                    // Add the rows to the node list
                    nodes.Add(startParentCell.ParentNode);

                    if (endParentCell.ParentNode != startParentCell.ParentNode) {
                        // Add all rows between the start & end
                        Node nextRow = startParentCell.ParentNode;

                        do {
                            nextRow = nextRow.NextSibling;
                            nodes.Add(nextRow);
                        }
                        while (nextRow != endParentCell.ParentNode && nextRow != null);
                    }
                }
            }
            else {
                // Region does not sit in a table, ensure the start & end sit at the same level in
                // the document tree
                Node startParentNode = regionStart.Start;
                Node endParentNode = regionEnd.End;

                // If the start & end nodes belong to different parents, it is common that they 
                // are both in their own Paragraph tags
                if (startParentNode.ParentNode != endParentNode.ParentNode)
                {
                    startParentNode = this.GetNodeParentParagraph(regionStart.Start);
                    endParentNode = this.GetNodeParentParagraph(regionEnd.Start);
                }
                else
                {
                    // Since the start & end are the actual merge fields, we need to change the
                    // start and end to be their siblings, since the merge fields will be removed
                    if (regionStart.End.NextSibling != regionEnd.Start) {
                        startParentNode = regionStart.End.NextSibling;
                        endParentNode = regionEnd.Start.PreviousSibling;
                    }
                }

                if (startParentNode.ParentNode != endParentNode.ParentNode) {
                    // No, so return an error
                    this.ErrorList.Add(new MergeError(
                        MergeErrorType.RegionStartAndEndDifferentParents,
                        region.Name,
                        "The start & end merge fields for the region " + region.Name + " are " + "in different levels of the document."));
                }
                else {

                    // Add all nodes between the merge fields to the node list                                        
                    Node currentNode = startParentNode;
                    while (currentNode != endParentNode && currentNode != null) {
                        nodes.Add(currentNode);
                        currentNode = currentNode.NextSibling;
                    }

                    if (currentNode != null)
                        nodes.Add(currentNode);
                }
            }

            // Finally, remove the merge fields as they are no longer needed, and they can cause
            // an infinite loop when recursing through regions
            if (region.Type == MergeRegionType.Conditional) {
                DocumentBuilder builder = new DocumentBuilder(Document);
                if (builder.MoveToMergeField(regionEnd.Name, true, false)) {                    
                    //builder.InsertField(@" MERGEFIELD " + regionEnd.Name.ToLower().Replace("ifend:", "IfTmp:"), @" MERGEFIELD " + regionEnd.Name.ToLower().Replace("ifend:", "IfTmp:"));
                }
                else {
                }
            }

            foreach (Node node in regionStart.Nodes) {
                if (node.GetType() == typeof(Run)) {
                    Run run = (Run)node;

                    // If the region is split across two runs
                    if (run.NextSibling != null) {
                        if (run.NextSibling.GetType() == typeof(Run)) {
                            Run runsibling = (Run)run.NextSibling;
                            run.Text += runsibling.Text;
                            runsibling.Text = string.Empty;
                        }
                    }

                    run.Text = run.Text.Replace("IfStart", "If");
                    run.Text = run.Text.Replace("TableStart", "Table");
                    run.Text = run.Text.Replace("ListFormatStart", "List");
                    run.Text = run.Text.Replace("Start", "");
                }
            }
            
            foreach (Node node in regionEnd.Nodes) {
                if (node.GetType() == typeof(Run)) {
                    Run run = (Run)node;
                    run.Text = run.Text.Replace("TableEnd", "Table");
                    run.Text.Replace("ListFormatEnd", "List");
                }
            }

            if (region.Type == MergeRegionType.Conditional) {
                // this.RemoveMergeField(regionEnd);
            }

            if (region.Type == MergeRegionType.Table) {
                this.RemoveMergeField(regionStart);
                this.RemoveMergeField(regionEnd);
            }

            
            if (region.Type == MergeRegionType.ListFormat) {
                DocumentBuilder builder = new DocumentBuilder(Document);
                builder.MoveToMergeField("List:" + region.Name, true, true);
                builder.MoveToMergeField("ListFormatEnd:" + region.Name, true, true);
            }
            
            return nodes;
        }

        public MergeRegion GetRegionFromStartNode(IEnumerable<MergeRegion> regions, Node startNode, MergeRegionType type) {

            // Is this recursion deep enough?
            MergeRegion foundRegion = null;
            foreach (MergeRegion region in regions) {
                if (region.Start == startNode) {
                    return region;
                }
            }
            return foundRegion;
        }

        private void MergeNodes(List<Node> nodes, DataRow rowData) {
            if (rowData != null) {
                MergeField mergeField;

                foreach (Node node in nodes) {
                    if (node.GetType() == typeof(FieldStart)) {
                        mergeField = this.GetMergeField((FieldStart)node);
                        this.SetMergeField(mergeField, rowData);
                    }
                }
            }
        }

        private string SubstituteMergeFieldName(MergeField mergeField,  DataRow row)
        {

            string result = mergeField.Name;
            result = FieldSubstitutebyArray(result);
            return result;

        }

        public System.Collections.Generic.List<string[,]> FieldSubstituteArray = new System.Collections.Generic.List<string[,]>();
        private string FieldSubstitutebyArray(string input)
        {
            string result = input;

            try
            {
                foreach (var item in FieldSubstituteArray)
                {
                    result = result.Replace( item[0,0], item[0,1].ToString());
                }
            }
            catch
            {
                // continue
            }

            return result;

        }

        private string FieldSubstitutebySwitch(MergeField mergeField)
        {

            var result = mergeField.Name;

            try
            {

                result = string.Format(mergeField.Name, mergeField.Switches);
                
            }
            catch
            {
                // continue
            }

            return result;

        }

        private string FieldSubstitutebyData(string input, DataRow row)
        {

            try
            {
                //Replace from MergeField Value


                // Replace From Data
                Regex Pattern = new Regex("{.*?}", RegexOptions.IgnoreCase); //TODO: Revert to {{}}
                var matches = Pattern.Matches(input);

                foreach (Match match in matches)
                {
                    input = ReplaceBracketValuesXML(match, row, input);
                }

            }
            catch
            {
                // continue
            }

            return input;

        }

        public static string GetElementValue(XElement File, string fldname)
        {
            try
            {
                XElement ElementMatch = File.Element(fldname);
                return ElementMatch != null ? ElementMatch.Value : "";
            }
            catch
            {
                return string.Empty;
            }

        }

        private void SetMergeField(MergeField mergeField, DataRow rowData) {

            try { 

                string mergeValue = "";

                Color debughighlight = Color.LightGreen;

                // Ensure this fields has not already been merged
                if (!mergeField.Merged) {
                    // Ensure this is a valid merge field
                    if (mergeField.Name != null) {
                        // Does a data column exist for this merge field?

                        string fieldname = SubstituteMergeFieldName(mergeField, rowData);

                        if (rowData != null) {

                            if (rowData.Table.Columns.Contains(fieldname)) {
                                mergeValue = Convert.ToString(rowData[fieldname]);
                            }
                            else if (this.DocumentData.Table.Columns.Contains(fieldname)) {
                                    mergeValue = Convert.ToString(this.DocumentData[fieldname]);
                            }
                            else if (fieldname.ToLower().StartsWith("next.")){

                                mergeValue = ParseRegionOrFieldValue(fieldname, rowData);

                            }                        
                            // Specfied Table
                            // used to merge a field where it is in the parent table
                            // I.E Name_rpt.Name_txt where it is contained in a child repeat
                            else if (fieldname.Contains(".") &&  !fieldname.ToLower().Contains("listformat:") && !fieldname.ToLower().Contains("if:") && !fieldname.ToLower().Contains("iftmp:") && !fieldname.ToLower().Contains("ifend:") && !fieldname.ToLower().Contains("ifstart:") && !fieldname.ToLower().Contains("ifstart")) {

                                String[] vals = fieldname.Split('.');
                                string tblname = vals[0];
                                string fldname = vals[1];

                                // get the parent ID of the table
                                string parentID = Convert.ToString(rowData["ParentID"]);

                                if (DocumentData.Table.DataSet.Tables.Contains(tblname)) {
                                    DataRow[] rows = DocumentData.Table.DataSet.Tables[tblname].Select("ID=" + parentID);
                                    if (rows.Length > 0) {
                                        if (rows[0].Table.Columns.Contains(fldname)) {
                                            mergeValue = Convert.ToString(rows[0][fldname]);
                                        }
                                    }
                                }
                            }
                            else {
                                // set the value into the field
                                if (fieldname.ToLower().StartsWith("if")) {

                                    if (debug) {

                                        ParseRegionValue(fieldname, out string regionName, out string regionFilter);

                                        string debugfiltervalues = string.Empty;
                                
                                        string filtercolumn = string.Empty;
                                        foreach (char c in fieldname) {

                                            if (c == ' ') filtercolumn = string.Empty;
                                            filtercolumn += c;

                                            var col =DocumentData.Table.Columns[filtercolumn.Trim()];

                                            if (rowData.Table.Columns.Contains(filtercolumn.Trim())) {
                                                debugfiltervalues = debugfiltervalues + " " + filtercolumn + "=" + rowData[filtercolumn.Trim()];
                                                filtercolumn = string.Empty;

                                                try {
                                                    if (rowData.Table.Select(regionFilter).Length == 0) {
                                                        debughighlight = Color.LightSalmon;
                                                    }
                                                }
                                                catch {
                                                    debughighlight = Color.LightSalmon;
                                                }

                                            }
                                        }

                                        if (string.IsNullOrEmpty(debugfiltervalues)) {
                                            debughighlight = Color.LightSalmon;
                                            debugfiltervalues = " Values=(can't find field in " + rowData.Table.TableName + " )";
                                        }
                                        else {
                                            debugfiltervalues= " Values=(" + debugfiltervalues.Trim() + ")";
                                        }
                                        if (fieldname.ToLower().Contains("ifend:")) {
                                            debughighlight = Color.Transparent;
                                            debugfiltervalues = string.Empty;
                                        }
                                        mergeValue = ("<<" + fieldname + debugfiltervalues + ">>");

                                    }

                                }
                            }
                        }
                        else {
                            // No, so look for the data at the document level
                            if (this.DocumentData.Table.Columns.Contains(fieldname)) {
                                mergeValue = Convert.ToString(this.DocumentData[fieldname]);
                            }
                        }

                        // Format the value
                        //mergeValue = MergeFieldFormatter.FormatValue(mergeField, mergeValue, new List<MergeRegionData>(), rowData);

                        // Is this an image merge field?
                        if (fieldname.StartsWith("Image:", StringComparison.CurrentCultureIgnoreCase) ) {
                            this.SetImageMergeField(mergeField, mergeValue, rowData);
                        }

                        if (mergeValue.ToLower().StartsWith("image/url")){
                            mergeValue = mergeValue.Replace("image/url", "");
                            mergeValue = mergeValue.Trim();

                            Uri uriResult;
                            bool result = Uri.TryCreate(mergeValue, UriKind.Absolute, out uriResult) ;

                            if (result)
                            {
                                using (var webClient = new WebClient())
                                {
                                    byte[] imageBytes = webClient.DownloadData(mergeValue);

                                    // Duplicated move to a function
                                    using (var stream = new MemoryStream(imageBytes, 0, imageBytes.Length))
                                    {

                                        System.Drawing.Image image = System.Drawing.Image.FromStream(stream, true);
                                        //image.Save("", System.Drawing.Imaging.ImageFormat.Png);
                                        //TODO: do something with image
                                        int width = image.Width; int height = image.Height;
                                        float output;
                                        bool resize = false;
                                        foreach (MergeFieldSwitch sw in mergeField.Switches)
                                        {
                                            if (sw.Name.ToLower() == "width")
                                            {
                                                if (float.TryParse(sw.Value, out output))
                                                {
                                                    width = Convert.ToInt16(sw.Value); resize = true;
                                                }
                                            }
                                            if (sw.Name.ToLower() == "height")
                                            {
                                                if (float.TryParse(sw.Value, out output))
                                                {
                                                    height = Convert.ToInt16(sw.Value); resize = true;
                                                }
                                            }
                                        }

                                        if (resize)
                                        {
                                            image = ResizeImage(image, new Size(width, height), false);
                                        }

                                        this.SetImage(mergeField, null, image);
                                        mergeField.Merged = true;
                                    }
                                }
                                mergeValue = "";
                            }

                        }

                        if ( mergeValue.ToLower().StartsWith("image/png") 
                            || mergeValue.ToLower().StartsWith("image/jpeg")
                            || mergeValue.ToLower().StartsWith("image/jpg")) {

                           // System.Drawing.Image img = new System.Drawing.Image();

                            mergeValue = mergeValue.Replace("image/png;base64,","");
                            mergeValue = mergeValue.Replace("image/jpeg;base64,", "");
                            mergeValue = mergeValue.Replace("image/jpg;base64,", "");

                            mergeValue = mergeValue.Replace(" ", "+");

                            byte[] data = Convert.FromBase64String(mergeValue);
                            using (var stream = new MemoryStream(data, 0, data.Length)) {

                                System.Drawing.Image image = System.Drawing.Image.FromStream(stream, true);
                                //image.Save("", System.Drawing.Imaging.ImageFormat.Png);
                                //TODO: do something with image
                                int width = image.Width; int height = image.Height;
                                float output;
                                bool resize = false;
                                foreach (MergeFieldSwitch sw in mergeField.Switches) {
                                    if (sw.Name.ToLower() == "width") {
                                        if (float.TryParse(sw.Value, out output)) {
                                            width = Convert.ToInt16(sw.Value); resize = true;
                                        }
                                    }
                                    if (sw.Name.ToLower() == "height") {
                                        if (float.TryParse(sw.Value, out output)) {
                                            height = Convert.ToInt16(sw.Value); resize = true;
                                        }
                                    }
                                }

                                if (resize) {
                                    image = ResizeImage(image, new Size(width, height)  , false);
                                }

                               
                                this.SetImage(mergeField,null, image );
                                mergeField.Merged = true;
                            }                        
                        }
                        else if (mergeValue.ToLower().StartsWith("image/svg")) {

                            // System.Drawing.Image img = new System.Drawing.Image();

                            mergeValue = "<svg height=\"100\" width=\"100\"><circle cx=\"50\" cy=\"50\" r=\"40\" stroke=\"black\" stroke-width=\"3\" fill=\"red\" /></svg> ";

                            var byteArray = Encoding.ASCII.GetBytes(mergeValue);
                            using (var stream = new MemoryStream(byteArray)) {
                                var svgDocument = Svg.SvgDocument.Open(stream);

                                var bitmap = svgDocument.Draw();
                                Image image = (Image)bitmap;

                                image = ResizeImage(image, new Size(100, 100), false);

                                this.SetImage(mergeField, null, image);
                                mergeField.Merged = true;
                            }


                        }

                        else if (fieldname.StartsWith("\"ss:", StringComparison.CurrentCultureIgnoreCase) || fieldname.StartsWith("ss:", StringComparison.CurrentCultureIgnoreCase)) {
                        }
                        else if (!string.IsNullOrEmpty(mergeValue) && mergeValue.Contains("\\rtf")) {
                            this.SetRTFMergeField(mergeField, mergeValue);
                        }
                        else {
                            this.SetTextMergeField(mergeField, mergeValue, rowData, debughighlight);
                        }
                    }
                }

            }
            catch (Exception ex) {
                this.ErrorList.Add(new MergeError(MergeErrorType.InvalidMergeField, "Setting Merge Field value error", ex.Message));
            }

}

        public Image ResizeImage(Image image, Size size, bool preserveAspectRatio = true) {
            
            int newWidth;
            int newHeight;
            if (preserveAspectRatio) {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float percentWidth = (float)size.Width / (float)originalWidth;
                float percentHeight = (float)size.Height / (float)originalHeight;
                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int)(originalWidth * percent);
                newHeight = (int)(originalHeight * percent);
            }
            else {
                newWidth = size.Width;
                newHeight = size.Height;
            }

            Image newImage = new Bitmap(newWidth, newHeight);

            using (Graphics graphicsHandle = Graphics.FromImage(newImage)) {
                graphicsHandle.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        private int RowIndex(DataRow row) {

            DataTable tbl = row.Table;

            int i = 0;
            int iRowIndex = 0;
            for (i = 0; i <= tbl.Rows.Count - 1; i++) {
                if (tbl.Rows[i].Equals(row)) {
                    iRowIndex = i;
                    break; // : might not be correct. Was : Exit For
                }
            }

            return iRowIndex;

        }

        private void SetImageMergeField(MergeField mergeField, string mergeValue, DataRow rowData) {

            // Has an event handler been provided?
            if (MergeImageField != null) {
                MergeImageFieldEventArgs args = new MergeImageFieldEventArgs
                {
                    Document = this.Document,
                    Field = mergeField,
                    Row = rowData,
                    FieldName = mergeField.Name.Replace("Image:", ""),
                    FieldValue = mergeValue
                };

                MergeImageField(this, args);

                Shape shape = new Shape(this.Document, ShapeType.Image)
                {
                    WrapType = WrapType.Inline
                };

                // Has an image file name been provided by the handler?
                if (args.ImageFileName != null) {
                    shape.ImageData.SetImage(args.ImageFileName);
                }
                else if (args.Image != null) {
                    shape.ImageData.SetImage(args.Image);
                }

                if (args.ImageFileName != null || args.Image != null) {
                    this.ResizeImage(args, shape);

                    // Replace the merge field with the image
                    mergeField.Start.ParentNode.InsertBefore(shape, mergeField.Start);
                }

                //this.RemoveMergeField(mergeField);
            }
        }

        private void SetImage(MergeField mergeField, DataRow rowData, System.Drawing.Image image) {

            if (image != null){
                Aspose.Words.DocumentBuilder builder = new DocumentBuilder(this.Document);
                builder.MoveTo(mergeField.Start);
            
                using (MemoryStream ms = new MemoryStream()) {
                    image.Save(ms, image.RawFormat);
                    Byte[] img = ms.ToArray();
                    builder.InsertImage(img);
                }
            }
         
        }

        private string WildCardReplace(string input, DataRow row)
        {

            try
            {

                Regex Pattern = new Regex("{.*?}", RegexOptions.IgnoreCase); //TODO: Revert to {{}}
                var matches = Pattern.Matches(input);

                foreach (Match match in matches)
                {
                    input = ReplaceBracketValuesXML(match, row, input);
                }
            }
            catch
            {
                // continue
            }

            return input;

        }

        private string ReplaceBracketValuesXML(Match Match, DataRow row, string File)
        {
            string fldname = Match.Value.Replace("{", "").Replace("}", "");
            string fldvalue = GetRowValue(row, fldname);
            return File.Replace(Match.Value, fldvalue);
        }

        public static string GetRowValue(DataRow row, string fldname)
        {
            try
            {
                if (row.Table.Columns.Contains(fldname))
                {
                    return Convert.ToString(row[fldname]);
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }

        }

        private void SetTextMergeField(MergeField mergeField, string mergeValue, DataRow rowData, Color debughighlight) {
          
            try {
                // Word only supports \r for line breaks
                mergeValue = mergeValue.Replace("\r\n", "\r");
                mergeValue = mergeValue.Replace("\n", "\r");
                mergeValue = mergeValue.Replace("\\n", "\r");
                mergeValue = mergeValue.TrimEnd('|');

                // Has an event handler been provided?
                if (MergeField != null)
                {
                    MergeFieldEventArgs args = new MergeFieldEventArgs
                    {
                        Document = this.Document,
                        Field = mergeField,
                        Row = rowData,
                        FieldName = mergeField.Name,
                        FieldValue = mergeValue,
                        TableName = rowData.Table.TableName,
                        Text = mergeValue
                    };

                    //this.MergeField(this, args);

                    mergeValue = args.Text;
                }

                // Format the value
                // Support for wild cards in field values
                if (mergeField.Name.ToLower() == "insert")
                {
                    try
                    {
                       foreach (Node nd in mergeField.Nodes)
                        {
                            if (nd is Run run)
                            {
                                Regex Pattern = new Regex("{.*?}", RegexOptions.IgnoreCase); //TODO: Revert to {{}}
                                var matches = Pattern.Matches(run.Text);

                                foreach (Match match in matches)
                                {
                                    run.Text = ReplaceBracketValuesXML(match, rowData, run.Text);
                                }
                            }
                        }
                    }
                    catch { }
                }

                if (mergeField.Name.ToLower() != "append" && mergeField.Name != "Insert" && mergeField.Name.ToLower() != "documentbreakpoint")
                {

                    MergeFieldFormatter.document = Document;
                    mergeValue = MergeFieldFormatter.FormatValue(mergeField, mergeValue, this.Data, this.RegionData, rowData);

                    DocumentBuilder builder = new DocumentBuilder(this.Document);

                    // Clear all run values within the merge field
                    Node parent = mergeField.Nodes[0]; // null;
                    mergeValue = String.Concat(mergeValue, "");
                    builder.MoveTo(parent);

                    if (!mergeField.Name.ToLower().Contains("listformat") && !mergeField.Name.ToLower().Contains("liststart") && !mergeField.Name.ToLower().Contains("listitem"))
                    {
                        foreach (Node nd in mergeField.Nodes)
                        {
                            if (nd is Run run)
                            {
                                run.Text = "";
                            }
                        }
                    }

                    if (mergeValue == "" && (!mergeField.isConditionalRegion && !mergeField.isListFormatRegion && !mergeField.isTableRegion && !mergeField.isListItem))
                    {
                        mergeValue += "WF:DELETETORIGHT";
                    }

                    if (debug) builder.Font.HighlightColor = debughighlight;

                    // check for highlighting
                    if (mergeValue.Contains("HL-Yellow"))
                    {
                        builder.Font.HighlightColor = Color.Yellow; mergeValue = mergeValue.Replace("HL-Yellow", "");
                    }
                    else if (mergeValue.Contains("HL-Blue"))
                    {
                        builder.Font.HighlightColor = Color.Blue; mergeValue = mergeValue.Replace("HL-Blue", "");
                    }
                    else if (mergeValue.Contains("HL-Red"))
                    {
                        builder.Font.HighlightColor = Color.Red; mergeValue = mergeValue.Replace("HL-Red", "");
                    }
                    else if (mergeValue.Contains("HL-Green"))
                    {
                        builder.Font.HighlightColor = Color.Green; mergeValue = mergeValue.Replace("HL-Green", "");
                    }
                    else if (mergeValue.Contains("HL-Gray"))
                    {
                        builder.Font.HighlightColor = Color.Gray; mergeValue = mergeValue.Replace("HL-Gray", "");
                    }
                    else if (mergeValue.Contains("HL-Silver"))
                    {
                        builder.Font.HighlightColor = Color.Silver; mergeValue = mergeValue.Replace("HL-Silver", "");
                    }
                    else if (mergeValue.Contains("HL-Black"))
                    {
                        builder.Font.HighlightColor = Color.Black; mergeValue = mergeValue.Replace("HL-Black", "");
                    }
                    else if (mergeValue.Contains("HL-Cyan"))
                    {
                        builder.Font.HighlightColor = Color.Cyan; mergeValue = mergeValue.Replace("HL-Cyan", "");
                    }
                    else if (mergeValue.Contains("HL-Orange"))
                    {
                        builder.Font.HighlightColor = Color.Orange; mergeValue = mergeValue.Replace("HL-Orange", "");
                    }

                    builder.Write(mergeValue);

                    if (mergeValue == "")
                    {
                        // The merged value is blank. Is this the only merge field in this paragraph?
                        CompositeNode parentNode = mergeField.Start.ParentNode;
                    }
                }


                mergeField.Merged = true;
            }
            catch (Exception ex) {
                this.ErrorList.Add(new MergeError(MergeErrorType.InvalidMergeField, "Setting Merge Field value error", ex.Message));
            }

}

        private void SetRTFMergeField(MergeField mergeField, string mergeValue) {
            try { 
               if (!string.IsNullOrEmpty(mergeValue) && mergeValue.Contains("\\rtf")) {
                Node parentNode = mergeField.Nodes[3].ParentNode; // mergeField.Nodes[1].ParentNode;

                // RTF documents uses ASCII encoding.
                // Find ANSI codepage (\\ansicpg1252).
                string CodePage = "", WindowsCodePage = "";
                Regex re = new Regex(@"\\ansicpg\d+");
                Match m = re.Match(mergeValue);
                if (m.Success) {
                    // Get codepage (1252)
                    Regex reCodepage = new Regex(@"\d+");
                    Match mCodePage = reCodepage.Match(m.Value);
                    if (mCodePage.Success) {
                        CodePage = mCodePage.Value;
                    }
                }

                byte[] rtfBytes = null;
                try {
                    if (!string.IsNullOrEmpty(CodePage)) {
                        WindowsCodePage = "Windows-" + CodePage; // "Windows-1252"
                        rtfBytes = Encoding.GetEncoding(WindowsCodePage).GetBytes(mergeValue);
                    }
                }
                catch (System.ArgumentException) {
                    // GetEncoding failed
                }
                finally {
                    if (rtfBytes == null || rtfBytes.Length == 0) {
                        rtfBytes = Encoding.UTF8.GetBytes(mergeValue);
                    }
                }

                // Insert RTF document into destination document
                MemoryStream rtfStream = new MemoryStream(rtfBytes);
                Document rtfDoc = new Document(rtfStream);
                InsertDocument(parentNode, rtfDoc);
            }
            }
            catch (Exception ex) {
                this.ErrorList.Add(new MergeError(MergeErrorType.InvalidMergeField, "Setting Merge Field value error", ex.Message));
            }

}

        /// <summary>
        /// Inserts content of the external document after the specified node.
        /// Section breaks and section formatting of the inserted document are ignored.
        /// </summary>
        /// <param name="insertAfterNode">Node in the destination document after which the content 
        /// should be inserted. This node should be a block level node (paragraph or table).</param>
        /// <param name="srcDoc">The document to insert.</param>
        /// 
        public void InsertDocument(Node insertAfterNode, Document srcDoc) {

            // Make sure that the node is either a pargraph or table.
            if ((!insertAfterNode.NodeType.Equals(NodeType.Paragraph)) & (!insertAfterNode.NodeType.Equals(NodeType.Table))) {
                throw new ArgumentException("The destination node should be either a paragraph or table.");
            }

            // We will be inserting into the parent of the destination paragraph.
            CompositeNode dstStory = insertAfterNode.ParentNode;

            // This object will be translating styles and lists during the import.
            NodeImporter importer = new NodeImporter(srcDoc, insertAfterNode.Document, ImportFormatMode.KeepSourceFormatting);

            // Loop through all sections in the source document.
            foreach (Section srcSection in srcDoc.Sections) {
                // Loop through all block level nodes (paragraphs and tables) in the body of the section.
                foreach (Node srcNode in srcSection.Body) {
                    // Let's skip the node if it is a last empty paragarph in a section.
                    if (srcNode.NodeType.Equals(NodeType.Paragraph)) {
                        Paragraph para = (Paragraph)srcNode;

                        if (para.IsEndOfSection && !para.HasChildNodes)
                            continue;
                    }

                    // This creates a clone of the node, suitable for insertion into the destination document.
                    Node newNode = importer.ImportNode(srcNode, true);

                    // Insert new node after the reference node.
                    dstStory.InsertAfter(newNode, insertAfterNode);
                    insertAfterNode = newNode;
                }
            }
        }

        private Shape ResizeImage(MergeImageFieldEventArgs args, Shape shape) {

            double sourceWidth = shape.ImageData.ImageSize.WidthPoints;
            double sourceHeight = shape.ImageData.ImageSize.HeightPoints;
            double destWidth = sourceWidth;
            double destHeight = sourceHeight;
            double nPercent = 0;
            double nPercentW = 0;
            double nPercentH = 0;

            if (args.WidthPoints > 0) {
                destWidth = args.WidthPoints;
            }

            if (args.HeightPoints > 0) {
                destHeight = args.HeightPoints;
            }

            if (args.MaintainAspectRatio) {
                nPercentW = (destWidth / sourceWidth);
                nPercentH = (destHeight / sourceHeight);

                if (nPercentH < nPercentW) {
                    nPercent = nPercentH;
                }
                else {
                    nPercent = nPercentW;
                }

                destWidth = (sourceWidth * nPercent);
                destHeight = (sourceHeight * nPercent);
            }

            shape.Width = destWidth;
            shape.Height = destHeight;

            return shape;
        }

        private List<Node> GetChildNodes(CompositeNode parentNode) {
            List<Node> nodes = new List<Node>();

            foreach (Node node in parentNode.ChildNodes) {
                nodes.Add(node);
            }

            return nodes;
        }

        private void RemoveRegion(MergeRegion region) {
            foreach (Node node in region.Nodes) {
                //Does this still work? Has the Parent Node been removed all ready?

                if(node != null)
                {
                    if (node.ParentNode != null) {
                        node.Remove();
                    }
                }

            }
        }

        private void RemoveMergeField(MergeField mergeField) {

            CompositeNode parentNode = mergeField.Start.ParentNode;
            foreach (Node node in mergeField.Nodes) {
                node.Remove();
            }

            // Is the parent node now an empty paragraph?
            if (parentNode.GetType() == typeof(Paragraph) && parentNode.ChildNodes.Count == 0) {
                // Yes, so flag it to be removed
                Run run = new Run(this.Document, "REMOVE_PARA");
                parentNode.AppendChild(run);
            }
        }

        private void RemoveRegion(MergeField mergeField) {

            CompositeNode parentNode = mergeField.Start.ParentNode;

            foreach (Node node in mergeField.Nodes) {
                node.Remove();
            }

            // Is the parent node now an empty paragraph?
            // Yes, so flag it to be removed
            Run run = new Run(this.Document, "REMOVE_PARA");
            parentNode.AppendChild(run);

        }

        public MergeField GetMergeField(FieldStart fieldStart) {

            MergeField mergeField = new MergeField();

            // A merge field is not a single node, but 5 separate nodes:
            // 1. FieldStart
            // 2. Run - this contains the value of the field
            // 3. FieldSeparator
            // 4. Run - this contains the 'friendly name' of the field
            // 5. FieldEnd

            // Check that this is a valid merge field
            // this.IsMergeField(fieldStart)
            if (IsMergeField(fieldStart)) {
                // Get the merge field name & switches

                // Add all merge field nodes
                mergeField.Nodes = new Collection<Node>();

                Collection<Node> mergeFieldNodes = new Collection<Node>();
                Node currentNode = fieldStart;
                string value = "";

                bool isend = false;
                try {
                    while ( currentNode != null && currentNode.GetType() != typeof(FieldEnd)  ) {
                        mergeFieldNodes.Add(currentNode);

                        currentNode = currentNode.NextSibling;
                        try {
                            if(currentNode != null) {
                                if (currentNode.NextSibling != null) {
                                    if (currentNode.NextSibling.GetType() != typeof(FieldEnd) && currentNode.GetType() != typeof(FieldEnd) && currentNode.GetType() != typeof(FieldSeparator)) {

                                        // To cater for strangly formatted mergefields in the template 
                                        // Need to still find the fieldend
                                        if (currentNode.Range.Text.Contains("«")) isend = true;
                                        if (!isend) value += currentNode.Range.Text;
                                    }
                                }
                            }


                        }
                        catch {
                            //this.ErrorList.Add(new MergeError(MergeErrorType.MissingRegionEnd, value, "Error " + value ));
                            //Handle The nulls, do nothing
                        }
                    }
                }
                catch (Exception)
                {
                    string valuerr = string.Empty;
                    ParseMergeFieldValue(value, out string nameerr, out IEnumerable<MergeFieldSwitch> switcheserr);

                    MergeError merr = new MergeError(MergeErrorType.InvalidMergeField, nameerr, "Error " + nameerr);
                    if ( ErrorList.Where( i => i.Node == merr.Node ).Count() == 0 ) {
                        ErrorList.Add(merr);
                    }
                    
                }

                ParseMergeFieldValue(value, out string name, out IEnumerable<MergeFieldSwitch> switches);

                mergeField.Start = fieldStart;
                mergeField.Name = name;
                //mergeField.Region = FindParentRegion(fieldStart);
                mergeField.Switches = switches;

                // Add the end node
                mergeFieldNodes.Add(currentNode);
                mergeField.End = (FieldEnd)currentNode;
                mergeField.Nodes = mergeFieldNodes;
            }

            return mergeField;
        }

        private bool IsMergeField(FieldStart fldstart) {

            bool validMergeField = false;

            // Ensure that this is a merge field and not a different field

            if (fldstart.NextSibling != null) {

                Node fldtocheck = fldstart.NextSibling;
                string fldval = fldtocheck.GetText().Trim();

                while (fldtocheck is Run) {
                    if (fldval.Trim().ToUpper().StartsWith ("MERGEFIELD")) {
                        validMergeField = true;
                        break;
                    }

                    if (fldtocheck.NextSibling != null) {
                        fldtocheck = fldtocheck.NextSibling;
                        fldval += fldtocheck.GetText().Trim(); 
                    }
                    else {
                        break;
                    }

                }

            }
            else {
                validMergeField = false;
            }

            return validMergeField;
        }

        private void ParseMergeFieldValue(string value, out string name, out IEnumerable<MergeFieldSwitch> switches) {

            name = null;
            switches = null;

            // The structure of a Word merge field value is as follows:
            // MERGEFIELD <<FieldName>> <<Switches>>
            // e.g MERGEFIELD  FirstName  \* MERGEFORMAT

            // Split the value to get the switches
            string[] components = value.Trim().Split(@"\".ToCharArray());

            // The first component contains the word MERGEFIELD, 
            // followed by the merge field label, separated by a space

            // Set the name
            name = components[0].Replace("MERGEFIELD", "").Trim();
            name = name.Replace("mergefield", "").Trim();

            if (name.Contains("«") || name.Contains("»")) {

            }

            Regex regex = new Regex("«.*?»", RegexOptions.IgnoreCase);
            name = regex.Replace(name, "").Trim();

            // Set the switches
            switches = this.ParseMergeFieldSwitches(components);
        }

        private void ParseRegionValue(string value, out string name, out string filter) {

            name = null;
            filter = null;

            // The structure of a Region is as follows:
            // MERGEFIELD <<RegionName>> <<Filter>>
            // e.g MERGEFIELD IfStart:Taxable Taxable = 1

            // Split the value to get the switches
            string[] components = value.Replace("\"", "").Trim().Split(":".ToCharArray());

            // The first component contains the word MERGEFIELD, 
            // followed by the merge field label, separated by a space

            // Set the name
            if (components.Length > 1) {
                string[] subComponents = components[1].Split(" ".ToCharArray());

                name = subComponents[0].Trim();

                if (subComponents.Length > 1) {
                    filter = "";

                    for (int i = 1; i < subComponents.Length; i++) {
                        if (i > 1)
                            filter += " ";

                        filter += subComponents[i].Trim();
                    }
                }
            }
        }

        private IEnumerable<MergeFieldSwitch> ParseMergeFieldSwitches(string[] components) {

            List<MergeFieldSwitch> switches = new List<MergeFieldSwitch>();
            string component;
            string name;
            string value;
            int valueIndex;

            if (components.Length > 1) {
                for (int i = 1; i < components.Length; i++) {
                    component = components[i].Trim();

                    // Does this component contain a value?
                    valueIndex = component.IndexOf(" ");

                    if (valueIndex > -1) {
                        name = component.Substring(0, valueIndex).Trim();
                        value = component.Substring(valueIndex + 1).Trim();

                        Regex regex = new Regex("«.*?»", RegexOptions.IgnoreCase);
                        value = regex.Replace(value, "");

                    }
                    else {
                        name = component.Trim();
                        value = "";
                    }

                    switches.Add(new MergeFieldSwitch(name, value));
                }
            }

            return switches;
        }

        private bool ValidateRegionFilter(MergeRegion region, MergeRegion parentRegion) {
            bool validFilter = true;

            //if (region.Filter != null){
            //    MergeRegionData regionData;
            //    DataTable data = null;

            //    if (region.Type == MergeRegionType.Conditional){
            //        // Build conditional DataTable for validation
            //        if (parentRegion != null){


            //            MergeRegion parenttableregion = FindParentTableRegion(parentRegion);
            //            DataRow dataRow = null;

            //            if (parenttableregion != null) {
            //                regionData = this.GetRegionData(parenttableregion.Name);


            //                if (regionData != null && regionData.Data.Rows.Count > 0) {
            //                    dataRow = regionData.Data.Rows[0];
            //                }
            //                else if (regionData != null) {
            //                    // There is no data for this region, so create a new row
            //                    dataRow = regionData.Data.NewRow();
            //                }

            //                if (dataRow != null || this.DocumentData != null) {
            //                    data = this.GetConditionalDataTable(dataRow);
            //                }
            //            }
            //            else {

            //                if (dataRow == null && this.DocumentData != null) {
            //                    data = this.GetConditionalDataTable(dataRow);
            //                }

            //            }


            //        }
            //    }
            //    else{
            //        regionData = this.GetRegionData(region.Name);

            //        if (regionData != null){
            //            data = regionData.Data;
            //        }
            //    }

            //    // Validate the filter against the data source
            //    // Always Valid
            //    if (data != null) {
            //        try {
            //            data.Select(region.Filter);
            //        }
            //        catch (Exception e) {
            //            this.ErrorList.Add(new MergeError(
            //                MergeErrorType.InvalidRegionFilter,
            //                region.Name,
            //                "The region " + region.Name + " has an invalid filter expression: " +
            //                e.Message));

            //            validFilter = false;
            //        }
            //    }
            //}

            return validFilter;
        }

        private MergeRegion FindParentTableRegion(MergeRegion parent) {

            MergeRegion retval = parent;
            if (parent != null) {
                if (parent.Type == MergeRegionType.Table) {
                    return parent;
                }
                else {
                    retval = FindParentTableRegion(parent.ParentRegion);
                }
            }

            return retval;
        }

        private MergeRegion FindParentRegion(Node parent) {

            MergeRegion retval = null;
            if (parent != null && parent.ParentNode != null) {
                if (parent.ParentNode.GetType() == typeof(Aspose.Words.Fields.FieldStart)) {

                    MergeField fld = GetMergeField((FieldStart)parent);
                    MergeField regionStart = this.FindMergeFieldByText(fld.Name);
                    retval = GetRegionFromStartNode(Regions, regionStart.Start, regionStart.Region.Type);
                }
                else {
                    FindParentRegion(parent.ParentNode);
                }
            }
            return retval;
        }


        private MergeField FindMergeFieldByText(string text) {

            NodeList fields = this.Document.SelectNodes("//FieldStart");
            MergeField mergeField = null;
            MergeField foundField;
            
            foreach (Node field in fields) {
                foundField = this.GetMergeField((FieldStart)field);
                if (foundField.Name != null) {

                    // String foundfldname = foundField.Name.Replace("Mergefield", "").Trim(' ');

                    string foundfldname = Regex.Replace(foundField.Name, "mergefield", "", RegexOptions.IgnoreCase).Trim(' ');

                    if (foundField.Name.Length == text.Length) {
                        foundfldname = foundField.Name.Substring(0, text.Length);
                    }
                    //if (foundField.Name.Contains(text))

                    if (foundfldname.ToLower() == text.ToLower()) {
                        mergeField = foundField;
                        break;
                    }
                }
            }

            return mergeField;
        }

        private MergeField FindIFRegionEnd(MergeField regionstart) {

            MergeField mergeField = null;
            MergeField foundField;

            int cntstart = 0;
            int cntend = 0;

            bool go = true;
            while (go) {

                bool start = false;

                foreach (Node field in FieldStarts) {

                    // This is where we start counting
                    if (start) {

                        foundField = this.GetMergeField((FieldStart)field);

                        if (foundField.Name != null) {

                            String foundfldname = foundField.Name;

                            if (foundField.Name.ToLower().StartsWith("ifstart")) {
                                cntstart++;
                            }

                            if (foundField.Name.ToLower().StartsWith("ifend")) {
                                cntend++;
                                if (cntend > cntstart) {
                                    // this is the field start end
                                    mergeField = foundField;
                                    go = false;
                                    break;
                                }
                            }

                        }
                    }

                    // Start the count
                    if (field == regionstart.Start) start = true;

                }

                go = false;

            }

            return mergeField;
        }

        private Aspose.Words.Fields.Field FindFieldByText(string text) {

            Field retval = null;
            NodeCollection fieldStarts = this.Document.GetChildNodes(NodeType.FieldStart, true);

            foreach (FieldStart fieldStart in fieldStarts) {
                String s = fieldStart.Range.Text;
                if (fieldStart.FieldType == FieldType.FieldMergeField) {
                    Console.WriteLine("hi");
                    //string fieldCode = GetFieldCode(fieldStart);t
                    // Here you can use fieldCode to extract any necessary substring.
                }
            }

            return retval;
        }

        private Cell GetNodeParentCell(Node node) {
            Cell cell = null;

            while (node.ParentNode != null) {
                if (node.ParentNode.GetType() == typeof(Cell)) {
                    cell = (Cell)node.ParentNode;
                    break;
                }

                node = node.ParentNode;
            }

            return cell;
        }

        private CompositeNode GetNodeParentParagraph(Node node) {
            CompositeNode para = null;

            while (node.ParentNode != null) {

                if (node.ParentNode.GetType() == typeof(Paragraph)) {
                    para = node.ParentNode;
                    break;
                }
                node = node.ParentNode;
            }

            return para;
        }
    }

}