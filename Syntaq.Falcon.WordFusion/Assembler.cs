// 3rd Party Using Statements
using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Tables;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;

// WordFusion Using
using WordFusion.Assembly.MailMerge;
using WordFusion.SystemFramework.DataSets;

namespace WordFusion.Assembly
{

    public class Assembler {

        #region Properties

        public event InsertHandlerEvent InsertHandler;

        private MailMerge.MailMerge MailMerge;

        public DataSet DataMergeText = new DataSet();
        private DataSet m_FormData = new DataSet();
        private readonly string m_tblMerging = "File"; //Redundant variable?

        public Collection<Exception> Errors = new Collection<Exception>();

        public Collection<string> Inserts = new Collection<string>();
        public Collection<string> InsertsWithHeaders = new Collection<string>();
        public Collection<string> Appends = new Collection<string>();

        private string m_Data;
        public string Data {
            get { return m_Data; }
            set { m_Data = value; }
        }

        public bool HasErrors {
            get {
                if (Messages.Messages.Rows.Count > 0) return true;
                return false;
            }
        }

        private bool _EvaluateIF = true;
        public Boolean EvaluateIF {
            get {
                return _EvaluateIF;
            }
            set {
                _EvaluateIF = value;
            }
        }

        [System.ComponentModel.DefaultValue(true)]
        public Boolean UpdateFields {
            get;
            set;
        }

        private Boolean _insertschedules = false;
        public Boolean InsertSchedules {
            get {
                return _insertschedules;
            }
            set {
                _insertschedules = value;
            }
        }
        private MessageDataSet m_Messages = new MessageDataSet();

        public MessageDataSet Messages {
            get { return m_Messages; }
            set { m_Messages = value; }
        }

        private string m_SaveFileName;
        public string SaveFileName {
            get { return m_SaveFileName; }
            set { m_SaveFileName = value; }
        }

        private string m_SaveFolder;
        public string SaveFolder {
            get { return m_SaveFolder; }
            set { m_SaveFolder = value; }
        }

        private string m_AnswerfName;
        public string AnswerfName {
            get { return m_AnswerfName; }
            set { m_AnswerfName = value; }
        }

        private WordFusion.SystemFramework.FileManager.DocumentTypes m_DocumentType;
        public WordFusion.SystemFramework.FileManager.DocumentTypes DocumentType {
            get { return m_DocumentType; }
            set { m_DocumentType = value; }
        }

        public String ReviewerID {
            get;
            set;
        }

        public DataSet SiteInfo {
            get;
            set;
        }

        # endregion

        # region Initialise

        public Assembler() {            
            TablesValidate();
        }

        private void TablesValidate() {

            foreach (DataTable tbl in DataMergeText.Tables) {
                if (!tbl.Columns.Contains("Order")) {

                    DataColumn col = new DataColumn("Order")
                    {
                        AutoIncrement = true,
                        ColumnMapping = MappingType.Attribute
                    };

                    tbl.Columns.Add(col);
                }
            }

        }

        # endregion

        # region Assemble

        # region Assemble

        private string CacheKey { get; set; }

         /// <summary>
        /// Primary Procedure to Assemble the Documents
        /// </summary>
        public Document Assemble(Document doc, string formdata, string cachekey = "") {

            //if (!string.IsNullOrEmpty(cachekey))
            //{
                CacheKey = cachekey.Trim();
            //}

            License license = new License();
            license.SetLicense("Aspose.Words.lic");

            // Set the Properties before AssemblingInsert
            Data =  formdata;

            // Create an XmlTextReader to read the file.
            // Read the XML document into the DataSet.
            // Close the XmlTextReader                
            TextReader reader = new StringReader(this.Data);
            m_FormData.ReadXml(reader);

            reader.Close();
            
            return Assemble(doc);

        }

        /// <summary>
        /// Primary Handler to Assemble the Documents
        /// </summary>
        private Document Assemble(Document doc) {

            try {

                //CultureInfo ci = new CultureInfo("en-US");
                //System.Threading.Thread.CurrentThread.CurrentCulture = ci;
                //try
                //{
                //    Configuration config = WebConfigurationManager.OpenWebConfiguration("/");
                //    GlobalizationSection section = (GlobalizationSection)config.GetSection("system.web/globalization");
                
                //    ci = string.IsNullOrEmpty(section.Culture) ? new CultureInfo("en-US") : new CultureInfo(section.Culture);               
                //    System.Threading.Thread.CurrentThread.CurrentCulture = ci;
                //}
                //catch(Exception ex)
                //{
                //    var zz = ex;
                //    //do we get here?
                //    // not sure how to check if web confi aviaible. using try catch atm
                //}


                if (doc != null) {

                    doc= AssembleDocument(doc);
                    if (Errors.Count == 0) {

                        // Make sure all Fields and TOCS are updated
                        if (UpdateFields) {
                            doc.UpdateFields();
                            doc.UpdateFields();
                        }

                        // Temporary Fix to workaround Reference issue
                        // To be removed when Aspose fixes
                        // i.e 3.1.1(a) > 3.1(a)
                        if (UpdateFields) {
                            //UpdateFieldReferences(doc);

                            try { doc.UpdatePageLayout(); }
                            catch { }
                        }
                    }
                    else {

                        // write the errors to the document

                        DocumentBuilder builder = new DocumentBuilder(doc);
                        builder.Font.Color = Color.Red;
                        builder.Font.Bold = true;
                        builder.Font.Size = 14;

                        builder.Write("--------------------" + Environment.NewLine);
                        builder.Write("ASSEMBLY ERROR LIST" + Environment.NewLine);
                        builder.Write("--------------------" + Environment.NewLine);
                        foreach (Exception ex in Errors) {
                            if (!(ex is NullReferenceException)) {
                                builder.Write(ex.Message + Environment.NewLine);
                            }
                        }
 
                    }

                    return doc;
                }
            }
            catch (Exception ex) {
                ErrorAdd(ex);
            }
            finally {

            }
            return null;
        }

        public void UpdateTOC(ref Document doc) {

            // Insert a table of contents at the beginning of the document.
            // IF a bookmark exists
            DocumentBuilder build = new DocumentBuilder(doc);
            if (build.MoveToBookmark("TOC_REF")) {
                //doc.Range.Bookmarks["TOC"].Text = "";
                build.InsertTableOfContents("\\o \"1-2\" \\h \\z \\u");
                build.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;
            };

        }

        public void UpdateDocumentFields(ref Document doc) {

            // Make sure all Fields and TOCS are updated
            try {
                doc.UpdateFields();
            }
            catch {
                // Continue processing
            }

            // Temporary Fix to workaround Reference issue
            // To be removed when Aspose fixes
            // i.e 3.1.1(a) > 3.1(a)
            //UpdateFieldReferences(doc);

            try { doc.UpdatePageLayout(); }
            catch {
                // Error in UpdatePageLayout
            }
            
        }

        private void UpdateFieldReferences(Document doc) {

            // Fixed in ASpose.Words 10.3
            return;

            //Get collection of fieldStarts from the document
            NodeCollection starts = doc.GetChildNodes(NodeType.FieldStart, true);

            //Loop though all field starts
            foreach (Aspose.Words.Fields.FieldStart start in starts) {
                //Check whether current field start is start of REF field
                if (start.FieldType == FieldType.FieldRef) {

                    //We should get field code and field value 
                    string fieldCode = string.Empty;
                    string fieldValue = string.Empty;
                    Node currentNode = start;
                    //Get Field code
                    while (currentNode.NodeType != NodeType.FieldSeparator) {
                        if (currentNode.NodeType == NodeType.Run)
                            fieldCode += (currentNode as Run).Text;
                        currentNode = currentNode.NextSibling;
                    }

                    //Get field value
                    while (currentNode.NodeType != NodeType.FieldEnd) {

                        if (currentNode.NodeType == NodeType.Run) {

                            fieldValue += (currentNode as Run).Text;

                            String val = "";
                            String[] vals = fieldValue.Split('.');

                            if (vals.Length > 2) {
                                for (int i = 1; i < vals.Length; i++) {
                                    val = val + vals[i] + ".";
                                }
                                val = val.TrimEnd('.');                            
                                (currentNode as Run).Text = val;
                            }
       
                            Node prevsibling = currentNode.PreviousSibling;
                            while (prevsibling.NodeType == NodeType.Run) {
                                (prevsibling as Run).Text = "";
                                prevsibling = prevsibling.PreviousSibling;
                            }
                            
                        }

                        currentNode = currentNode.NextSibling;
                    }
                }
            }

        }

        public static void UpdateRefFields(CompositeNode node)
        {
            // The document that this node belongs to.
            Document doc = (Document)node.Document;

            // Update list labels so we can get the list numbers of the paragraphs.
            doc.UpdateListLabels();

            // Each list level is represented by special symbols in the NumberFormat property. By referencing the index of the
            // list level the corresponding symbol can be retrieved.
            string[] listCodes = { "\0", "_, " , "0", "@", "P", "`", "\a", "\b" };

            // Iterate through all FieldRefs in the Document or CompositeNode. CompositeNode means that this code
            // can run on only certain parts of the document e.g a specific section.
            foreach (FieldStart start in node.GetChildNodes(NodeType.FieldStart, true))
            {
                if (start.FieldType == FieldType.FieldRef)
                {
                    // Parse the FieldCode.
                    Regex regex = new Regex(@"\s*(?<code>\S+)\s+(?<bookmark>\S+)\s+(?<switches>.+)");
                    Match match = regex.Match(GetFieldCode(start));

                    // See if the field contains the switch we are interested in.
                    if (match.Groups["switches"].Value.Contains("\\w"))
                    {
                        // Get the paragraph referenced by this field.
                        Bookmark bookmark = doc.Range.Bookmarks[match.Groups["bookmark"].Value];

                        if (bookmark != null)
                        {
                            // Get the paragraph that the reference bookmark is contained in.
                            Paragraph bookmarkPara = (Paragraph)bookmark.BookmarkStart.ParentNode;

                            // Get the current field result.
                            string fieldResult = GetFieldResult(start);

                            // Get the list number of the paragraph which is the proper result for this switch.
                            StringBuilder labels = new StringBuilder();

                            // Add the current list levels which are included by this list label.
                            int currentLevel = bookmarkPara.ListFormat.ListLevelNumber;
                            string baseLevelFormat = bookmarkPara.ListFormat.ListLevel.NumberFormat;
                            labels.Append(bookmarkPara.ListLabel.LabelString);

                            Node currentNode = bookmarkPara;

                            // Iterate through previous paragraphs to find the paragraph of each previous list level
                            // Stop if all levels have been processed.
                            while (currentNode != null && currentLevel >= 0)
                            {
                                if (currentNode.NodeType == NodeType.Paragraph)
                                {
                                    Paragraph para = (Paragraph)currentNode;
       
                                    // This paragraph belongs to the same list as the ref field as the bookmark para and is the current level we are looking for.
                                    if (para.ListFormat.ListLevelNumber == currentLevel && para.ListFormat.List == bookmarkPara.ListFormat.List)
                                    {
                                        // We now have the current levels list format. Since we are working backward to find the list strings we
                                        // need to insert this at the front of the result.

                                        // If the current listlabel we have calculated already includes this level then we don't need to include it again.
                                        if (!baseLevelFormat.Contains(listCodes[currentLevel]))
                                        {
                                            // These list labels are now included by this label. Add them to the base level so they are not included again.
                                            baseLevelFormat += para.ListFormat.ListLevel.NumberFormat;
                                            labels.Insert(0, para.ListLabel.LabelString);
                                        }

                                        // Search for the next list level down.
                                        currentLevel = para.ListFormat.ListLevelNumber - 1;
                                    }
                                }

                                // Search for paragraphs backward.
                                currentNode = currentNode.PreviousSibling;
                            }

                            // Replace the current field result with the new result. This should retain formatting.
                            start.ParentParagraph.Range.Replace(fieldResult, labels.ToString().TrimEnd('.'), false, false);
                        }
                    }
                }
            }
        }
        private static string GetFieldCode(FieldStart fieldStart){

            var builder = new StringBuilder();


            for (Node node = fieldStart; node != null && node.NodeType != NodeType.FieldSeparator && node.NodeType != NodeType.FieldEnd; node = node.NextPreOrder(node.Document)){

                // Use the text only of Run nodes to avoid duplication.

                if (node.NodeType == NodeType.Run)
                    builder.Append(node.GetText());

            }

            return builder.ToString();

        }

        private static string GetFieldResult(FieldStart fieldStart) {

            StringBuilder builder = new StringBuilder();

            bool isAtSeparator = false;
            for (Node node = fieldStart; node != null && node.NodeType != NodeType.FieldEnd; node = node.NextPreOrder(node.Document)) {

                if (node.NodeType == NodeType.FieldSeparator) {

                    isAtSeparator = true;

                }

                // Use text only of Run nodes to avoid duplication.

                if (isAtSeparator && node.NodeType == NodeType.Run)

                    builder.Append(node.GetText());
            }

            return builder.ToString();

        }

        public bool IsLocalAssembly {
            get;
            set;
        }

        public DataSet UserData {
            get;
            set;
        }

        public Image UserLogo {
            get;
            set;
        }

        private Document m_doc = new Document();

        private Document AssembleDocument(Document doc) {

            try {

                m_doc = doc;


                // Insert User Fields
                if (HasMailMergeFields(doc)) {
                    DocumentBuilder builder = new DocumentBuilder(doc);

                    // This will default as the Serverside UserLogo
                    Image logo = UserLogo;
                    if (logo != null) {
                        try {
                            while (builder.MoveToMergeField("User:Logo" , true, true)) {
                                builder.InsertImage(logo);
                            }
                        }
                        catch { // Continue regardless
                        }
                    }

                    //UserInfoMerge(doc); // data is preloaded into infofile
                    ReviewerInfoMerge(doc);
                    SiteInfoMerge(doc);

                }

 

                DocumentLoad(doc, m_FormData);


                if (Errors.Count > 0) {
                    return doc;
                }

                this.MailMerge.Merge();

                foreach (MergeError err in this.MailMerge.Errors)
                {
                    Errors.Add(new Exception(err.Type + ", " + err.Description));
                }

                MailMerge_Appends(doc);
                doc = MailMerge.Document;

                MergeInserts(doc);
                MergeStyles(doc);

                SetCheckBoxes(doc);

                // Process the errors
                //this.MailMerge.Errors

                // Insert any Optional Documents     
                if (InsertSchedules) {
                    doc = DocumentsAppend(doc);
                }

                DocumentsInsert(doc);
               
                DelEmptyTableRowsCols(doc);
                
                // Insert a table of contents at the beginning of the document.
                // IF a bookmark exists
                Aspose.Words.DocumentBuilder build = new DocumentBuilder(doc);
                if (build.MoveToBookmark("TOC")) {
                    //doc.Range.Bookmarks["TOC"].Text = "";
                    build.InsertTableOfContents("\\o \"1-2\" \\h \\z \\u");
                    build.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;
                };

                if (HasMailMergeFields(doc)) {
                    doc.MailMerge.DeleteFields();
                }
 
                DocumentCleanDown(doc);

                SetDocumentBreakpointBookmarks(doc);

                return doc;
            }
            catch (IOException ioex) {
                throw ioex;
            }
            catch (Exception ex) {

                foreach (MergeError error in this.MailMerge.Errors) {
                    ErrorAdd(new Exception(error.Description));
                }

                ErrorAdd(ex);
            }
            finally {

            }
            return null;
        }

        private void SetDocumentBreakpointBookmarks(Document doc)
        {

            // ErrorAdd bookmarks for splitting document in post processing 
            // Add Split BookMarks

            if (doc.Range.Text.ToLower().Contains("wf:documentbreakpoint"))
            {

                FindMatchedNodes startnodes = new FindMatchedNodes();
                doc.Range.Replace(new Regex(@"wf:documentbreakpointstart", RegexOptions.IgnoreCase), startnodes, true);
                FindMatchedNodes endnodes = new FindMatchedNodes();
                doc.Range.Replace(new Regex(@"wf:documentbreakpointend", RegexOptions.IgnoreCase), endnodes, true);

                int index = 0;
                DocumentBuilder builder = new DocumentBuilder(doc);
                foreach (Node nd in startnodes.nodes)
                {
                    builder.MoveTo(nd);
                    builder.StartBookmark("DocumentBreakPoint" + index);
                    builder.MoveTo((Node)endnodes.nodes[index]);
                    builder.EndBookmark("DocumentBreakPoint" + index);
                    index++;

                }
                doc.Range.Replace(new Regex(@"wf:documentbreakpointstart", RegexOptions.IgnoreCase), "");
                doc.Range.Replace(new Regex(@"wf:documentbreakpointend", RegexOptions.IgnoreCase), "");

            }

        }

        private void SetCheckBoxes(Document doc) {

            try {
                //DataTable tbl = m_FormData.Tables["File"];

                ////Loop throught all checkboxes in the table and set values
                //foreach (FormField field in doc.Range.FormFields) {

                //    var fldname = field.Name;

                //    if (tbl.Columns.Contains(fldname)) {

                //        var fldvalue = tbl.Columns[fldname].ToString();
                //        bool istrue = false;
                //        Boolean.TryParse(fldvalue, out istrue);
                //        if (istrue)
                //            field.Checked = true;
                //        else
                //            field.Checked = false;
                //    }

                //}

                
                //NodeCollection formfields = doc.GetChildNodes(NodeType.FormField, true);
                //foreach (FormField formfield in formfields) {

                //    if (formfield.Type == FieldType.FieldFormCheckBox) {

                //        var fldname = formfield.Name;

                //        if (tbl.Columns.Contains(fldname)) {

                //            var fldvalue = tbl.Columns[fldname].ToString();
                //            bool istrue = false;
                //            Boolean.TryParse(fldvalue, out istrue);
                //            formfield.Result = istrue ? "1" : "0";
                //        }

                //        break;
                //    }
                //}

                //    var nodes = doc.SelectNodes("//Form");

                //    NodeCollection paragraphs = doc.GetChildNodes(NodeType.Paragraph, true); 
                //    foreach (Paragraph para in paragraphs) {

                //            FormFieldCollection formfields = para.Range.FormFields;

                //            foreach (FormField formfield in formfields) {
                //                if (formfield.Type == FieldType.FieldFormCheckBox) {


                //                formfield.Result = "1"; // check ? "1" : "0";


                //                }
                //            }


                //    }



            }
            catch { }

        }
 

        private void DocumentsInsert(Document dstdoc) {

            //if (hasMailMergeFields(dstdoc)) {
            //    var v = 1;
            //}

            ////mainDoc.Range.Replace(new Regex("\\[MY_DOCUMENT\\]"), new InsertDocumentAtReplaceHandler(), false);
            //FindMatchedNodes obj = new FindMatchedNodes();
            //Regex regex = new Regex("\\[Insert .*\\]", RegexOptions.IgnoreCase);
            //dstdoc.Range.Replace(regex, obj, true);

            //while (obj.nodes.Count > 0) {

            //    foreach (Node node in obj.nodes) {
            //        //Your code
            //        node.Remove();
            //    }

            //    regex = new Regex("\\[Insert .*\\]", RegexOptions.IgnoreCase);
            //    dstdoc.Range.Replace(regex, obj, true);


            //}
            //// DocumentInsert(dstdoc, srcdoc, insert);

            //string start = Regex.Escape("<field=");
            //string format = Regex.Escape("format=");
            //string end = Regex.Escape(">");
            //string quote = Regex.Escape("”");

            //string regexStr = start + quote + @"(?<name>[^ \t\n\r\f\v>]+)" + quote +
            //        @"\s?(" + format + quote + "(?<format>[^ \t\n\r\f\v>]+)" + quote + ")?" + end;
            //Regex regex = new Regex(regexStr);

            //dstdoc.Range.Replace(regex, new ReplaceEvaluator(ReplaceTags), false);

        }

        //private ReplaceAction ReplaceTags(object sender, ReplaceEvaluatorArgs e) {
        //    //Get matched values
        //    string fieldName = e.Match.Groups["name"].Value;
        //    string fieldFormat = e.Match.Groups["format"].Value;

        //    string replacement = string.Empty;

        //    switch (fieldName) {
        //        case "ILLUSTRATION_REF":
        //            replacement = "this is my replacement";
        //            break;
        //        case "EXPIRY_DATE":
        //            replacement = DateTime.Now.ToString(fieldFormat);
        //            break;
        //    }

        //    e.Replacement = replacement;
        //    return ReplaceAction.Replace;
        //}

        //get the column and row index needed to be deleted, before delete any nodes 
        private void DelEmptyTableRowsCols(Document doc) {

            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);

            Collection<Row> rowstodelete = new Collection<Row>();
            Collection<int> cellindexs = new Collection<int>();

            //Loop through all tables
            for (int tableIndex = 0; tableIndex < tables.Count; ) {

                Table tbl = tables[tableIndex] as Table;
                //iterate row in each table
                foreach (Row row in tbl.Rows) {
                    string rowtext = row.GetText().Replace("\a", "");
                    if (rowtext.ToLower().Contains("wf:delrow")) {
                        rowstodelete.Add(row);
                    }

                    if (rowtext.ToLower().Contains("wf:delcol".ToLower()))
                    {
                        foreach (Cell cell in row.Cells)
                        {
                            string celltext = cell.GetText().Replace("\a", "");
                            if (celltext.ToLower().Contains("wf:delcol"))
                            {
                                cellindexs.Add(row.Cells.IndexOf(cell));
                            }
                        }
                    }
                }

                //delete row in each table
                foreach (Row row in rowstodelete) {
                    row.Remove();
                }
                rowstodelete.Clear();

                //delete column in each row
                foreach (Row row in tbl.Rows)
                {
                    foreach (int i in cellindexs) //delete mutiple col can cause problem
                    {
                        row.Cells[i].Remove();
                    }

                }
                cellindexs.Clear();

                tableIndex++;//Increase index
            }
        }
       
        private void DocumentLoad(Document Doc, DataSet data) {

            License license = new License();
            license.SetLicense("Aspose.Words.lic");

            this.MailMerge = this.MailMerge == null ? new MailMerge.MailMerge() : this.MailMerge;
            MailMerge.InsertHandler += MailMerge_InsertHandler;

            //if (MailMergeCache.MailMergeCacheItems.Any(i => i.Key.ToLower() == CacheKey.ToLower()) && ! string.IsNullOrEmpty(CacheKey))
            //{
            //    this.MailMerge = (WordFusion.Assembly.MailMerge.MailMerge)ObjectExtension.CloneObject(MailMergeCache.MailMergeCacheItems.First(i => i.Key.ToLower() == CacheKey.ToLower()).MailMerge);
            //    Doc = this.MailMerge.Document;
            //}
            //else
            //{
            //    this.MailMerge.Load(Doc);
            //    if (!string.IsNullOrEmpty(CacheKey))
            //    {
            //        MailMergeCache.MailMergeCacheItems.Add(new MailMergeCacheItem() { Key = CacheKey.ToLower(), DateUpdated = DateTime.Now, MailMerge = (WordFusion.Assembly.MailMerge.MailMerge)ObjectExtension.CloneObject(this.MailMerge) });
            //    }
                
            //}

            MailMerge.Load(Doc);

            MailMerge.Data = data;
            if (data.Tables["File"] != null) {
                MailMerge.DocumentData = data.Tables["File"].Rows[0];
            }

            foreach (DataTable tbl in data.Tables) {
                string tblName = tbl.TableName;
                if (tblName != "File") {
                    MailMerge.AddRegionData(data.Tables[tblName], tblName, "ID", "ParentID");
                }
            }

            foreach (DataTable tbl in DataMergeText.Tables) {
                string tblName = tbl.TableName;
                MailMerge.AddRegionData(tbl, tblName);
            }

            // Add a MergeField event
            MailMerge.MergeField += DocumentMergeFieldEventHandler;

            foreach (MergeError error in MailMerge.Errors) {
                ErrorAdd(new Exception(error.Description));
            }


 
        }

        public string[,] FieldSubstituteArray; 
        // Process inserts
        private void MergeInserts(Document document) {

            try {

                DocumentBuilder builder = new DocumentBuilder(document);
                MailMerge.MailMerge mmerge = new MailMerge.MailMerge();
                //bool hasinserts = false;

                while (builder.MoveToMergeField("Insert", false, false)) {
                    //hasinserts = true;

                    MergeField mergeField = mmerge.GetMergeField((FieldStart)builder.CurrentNode);

                    Assembler assemblerinsert = new Assembler
                    {
                        IsLocalAssembly = false,
                        UserLogo = UserLogo,
                        UserData = UserData,
                        DataMergeText = DataMergeText,
                        EvaluateIF = true,
                        ReviewerID = ReviewerID,
                        UpdateFields = true
                    };

                    // Add the list of inserts // VBCollection does not convert so add items here
                    if (Inserts != null) foreach (String insert in Inserts) { assemblerinsert.Inserts.Add(insert); }
                    if (InsertsWithHeaders != null) foreach (String insert in InsertsWithHeaders) { assemblerinsert.InsertsWithHeaders.Add(insert); }
                    if (Appends != null) foreach (String insert in Appends) { assemblerinsert.Appends.Add(insert); }

                    assemblerinsert.InsertSchedules = true;
                    assemblerinsert.User = this.User;

                    char[] charsToTrim = {'\\', ' ', '"'};

                    System.Collections.Generic.List<string[,]> Array = new System.Collections.Generic.List<string[,]>();
                    foreach (MergeFieldSwitch sw in mergeField.Switches)
                    {

                        try
                        {
                            if (sw.Name.ToLower() == "fieldsub")
                            {
                                var jsonstr = @"[" + sw.Value.Trim(charsToTrim) + "]";
                                JArray json = JArray.Parse(jsonstr);
                                foreach (JObject item in json)
                                {
                                    foreach (System.Collections.Generic.KeyValuePair<string, JToken> keyValuePair in item)
                                    {
                                        Array.Add(new string[1, 2] { { keyValuePair.Key, keyValuePair.Value.ToString() } });
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }                        
                    }

                    assemblerinsert.MailMerge = new MailMerge.MailMerge();
                    assemblerinsert.MailMerge.FieldSubstituteArray = Array;

                    foreach (MergeFieldSwitch sw in mergeField.Switches) {

                                if (sw.Name.ToLower() == "insert")
                                {
                                    string formurl = sw.Value.Trim(charsToTrim);

                                // Load Document from Cache
                                //Byte[] bydoc = DataHelper.FileDocumentGet(formurl, true);

                                Byte[] bydoc = GetDocumentByUrl(formurl);
                        
                                if (bydoc != null) {

                                    // Load Document from Cache
                                    Document docinsert = null;
                                    docinsert = new Document(new MemoryStream(bydoc));

                                    // Get the replacement from the field switch


                                    docinsert = assemblerinsert.Assemble(docinsert, this.Data, string.Empty);
                                    InsertDocumentAtMergeField(builder, document, docinsert);

                                }

                            }

                            foreach (Node nd in mergeField.Nodes) {
                                if (nd is Run run)
                                {
                                    run.Text = "";
                                }
                            }
                        }

                        

                }

                //if (hasinserts)
                //{
                //    LoadToCache(CacheKey);
                //}

            }
            catch (Exception ex) {
                ErrorAdd(ex);
            }

        }

        private string getStyleDocumentId()
        {

            string result = string.Empty;

            if (m_FormData.Tables.Contains("File"))
            {
                DataTable tbl = m_FormData.Tables["File"];
                if (tbl.Columns.Contains("StyleDocumentId"))
                {
                    DataRow row = tbl.Rows[0];
                    if (row != null)
                    {
                        result = Convert.ToString(row["StyleDocumentId"]);
                    }
                }
            }

            return result;

        }   

        private void MergeStyles(Document targetDoc)
        {

            try
            {   
                String StyleDocumentId = getStyleDocumentId();
                if (! string.IsNullOrEmpty(StyleDocumentId))
                {

                    var bydoc = GetDocumentByUrl(StyleDocumentId); // new Aspose.Words.Document("C:\\Users\\bfall\\Desktop\\Style2.docx");
                    if(bydoc != null)
                    {
                        Document srcDoc = new Document(new MemoryStream(bydoc));

                        foreach (Style style in srcDoc.Styles)
                        {
                            if (style.Type == StyleType.List || style.Type == StyleType.Paragraph || style.Type == StyleType.Table)
                            {
                                if (targetDoc != null)
                                {
                                    if (style.Type == StyleType.Paragraph || style.Type == StyleType.Table)
                                    {
                                        Style copied = targetDoc.Styles.AddCopy(style);
                                        copied.Name = style.Name;
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch { }
        }

        private void ReplaceHeaders(Document targetDoc)
        {

            try
            {
                String StyleDocumentId = getStyleDocumentId();
                if (!string.IsNullOrEmpty(StyleDocumentId))
                {

                    var bydoc = GetDocumentByUrl(StyleDocumentId); // new Aspose.Words.Document("C:\\Users\\bfall\\Desktop\\Style2.docx");
                    if (bydoc != null)
                    {
                        Document srcDoc = new Document(new MemoryStream(bydoc));

                        foreach (Style style in srcDoc.Sections)
                        {

                        }
                    }
                }

            }
            catch { }
        }

        public string RequestAuthority { get; set; }
        private Byte[]  GetDocumentByUrl(string url)
        {

            Byte[] bytedoc = null;
            using (WebClient client = new WebClient())
            {
                try
                {
                    // If only given an ID then // build the default uri
                    Guid outguid; bool isguid = Guid.TryParse(url, out outguid);
                    if (isguid)
                    {
                        url = RequestAuthority + "/Falcon/Templates/gettemplate?OriginalId=" + url + "&version=live";
                    }

                    bytedoc = new WebClient().DownloadData(url);
                }
                catch (Exception)
                {
                    bytedoc = null;
                }

            }

            return bytedoc;

        }

        private string WildCardReplace(string input)
        {

            try
            {
                XElement xdata = XElement.Parse(this.Data);
            
                Regex Pattern = new Regex("{.*?}", RegexOptions.IgnoreCase); //TODO: Revert to {{}}
                var matches = Pattern.Matches(input);

                foreach (Match match in matches)
                {
                    input = ReplaceBracketValuesXML(match, xdata, input);
                }
            }
            catch
            {
                // continue
            }

            return input;
 
        }

        private  string ReplaceBracketValuesXML(Match Match, XElement Data, string File)
        {
            string fldname = Match.Value.Replace("{", "").Replace("}", "");
            string fldvalue = GetElementValue(Data, fldname);
            return File.Replace(Match.Value, fldvalue);
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

        public void LoadToCache(string fpath)
        {
            var isdoc = new FileInfo(fpath).Extension.ToLower() == ".doc" || new FileInfo(fpath).Extension.ToLower() == ".docx" ? true : false;
            //var ispdf = new System.IO.FileInfo(fpath).Extension.ToLower() == ".pdf"? true:false;
            //var ishtml = new System.IO.FileInfo(fpath).Extension.ToLower() == ".html" ? true : false;
            if (isdoc)
            {
                string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority;
                string cachekey = (url + MapURL(fpath)).ToLower().Trim();

                Document doc = new Document(fpath);

                MailMerge.MailMerge mailmerge = new MailMerge.MailMerge();
                mailmerge.Load(doc);

                if (MailMergeCache.MailMergeCacheItems.Any(i => i.Key == cachekey))
                {
                    MailMergeCache.MailMergeCacheItems.Remove(MailMergeCache.MailMergeCacheItems.First(i => i.Key == cachekey));
                    MailMergeCache.MailMergeCacheItems.Add(new MailMergeCacheItem() { Key = cachekey, DateUpdated = DateTime.Now, MailMerge = (MailMerge.MailMerge)ObjectExtension.CloneObject(mailmerge) });
                }
                else
                {
                    MailMergeCache.MailMergeCacheItems.Add(new MailMergeCacheItem() { Key = cachekey, DateUpdated = DateTime.Now, MailMerge = (MailMerge.MailMerge)ObjectExtension.CloneObject(mailmerge) });
                }
            }

        }

        private string MapURL(string path)
        {
            string appPath = System.Web.Hosting.HostingEnvironment.MapPath("/").ToLower();
            return string.Format("/{0}", path.ToLower().Replace(appPath, "").Replace(@"\", "/"));
        }


        // Process inserts
        private void MailMerge_Appends(Document document) {

            try {

                DocumentBuilder builder = new DocumentBuilder(document);
                MailMerge.MailMerge mmerge = new MailMerge.MailMerge();

                while (builder.MoveToMergeField("Append", false, false)) {

                    MergeField mergeField = mmerge.GetMergeField((FieldStart)builder.CurrentNode);

                    Assembler assemblerinsert = new Assembler
                    {
                        IsLocalAssembly = false,
                        UserLogo = UserLogo,
                        UserData = UserData,
                        DataMergeText = DataMergeText,
                        EvaluateIF = true,
                        ReviewerID = ReviewerID,
                        UpdateFields = true
                    };

                    // Add the list of inserts // VBCollection does not convert so add items here
                    if (Inserts != null) foreach (String insert in Inserts) { assemblerinsert.Inserts.Add(insert); }
                    if (InsertsWithHeaders != null) foreach (String insert in InsertsWithHeaders) { assemblerinsert.InsertsWithHeaders.Add(insert); }
                    if (Appends != null) foreach (String insert in Appends) { assemblerinsert.Appends.Add(insert); }

                    assemblerinsert.InsertSchedules = true;
                    assemblerinsert.User = this.User;

                    foreach (MergeFieldSwitch sw in mergeField.Switches) {

                        string formurl = sw.Value;
                        // Load Document from Cache
                        Byte[] bydoc = DataHelper.FileDocumentGet(formurl, true);

                        if (bydoc != null) {
                            // Load Document from Cache
                            Document docinsert = null;
                            docinsert = new Document(new MemoryStream(bydoc));
                            docinsert = assemblerinsert.Assemble(docinsert, this.Data, string.Empty);

                            document.AppendDocument(docinsert, ImportFormatMode.KeepSourceFormatting);
                        }

                    }

                    foreach (Node nd in mergeField.Nodes) {
                        if (nd is Run run)
                        {
                            run.Text = "";
                        }
                    }

                }
            }
            catch (System.Net.WebException webex) {
                ErrorAdd(webex);
            }
            catch (Exception ex) {
                ErrorAdd(ex);
            }

        }

        private void MailMerge_InsertHandler(Document document, MergeField mergeField, string switchValue) {
          
        }

        private void DocumentMergeFieldEventHandler(object sender, MergeFieldEventArgs e) {
        }

        /// </summary>
        public static int StringCount(string text, string pattern) {

            // Loop through all instances of the string 'text'.
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1) {
                i += pattern.Length;
                count++;
            }
            return count;
        }

        private void DocumentCleanDown(Document doc) {

            doc.MailMerge.DeleteFields();

            try
            {
                doc.UpdateFields();
            }
            catch
            {
                // Continue on unknown ASPOSE issue
            }

            doc.Range.Replace(new Regex(@"<wf:Delete.*wf:Delete>", RegexOptions.IgnoreCase), "");
            doc.Range.Replace(new Regex(@"wf:DeleteToRight ", RegexOptions.IgnoreCase), "");
            doc.Range.Replace(new Regex(@"wf:DeleteToRight", RegexOptions.IgnoreCase), "");
            doc.Range.Replace(new Regex(@"wf:NumberText", RegexOptions.IgnoreCase), "");
            doc.Range.Replace(new Regex(@"wf:DollarText", RegexOptions.IgnoreCase), "");

            ReplaceSectionInsertNotLast insnotlast = new ReplaceSectionInsertNotLast();
            doc.Range.Replace(new Regex(@"WF:SectionInsertNotLast", RegexOptions.IgnoreCase), insnotlast, true);
            DocumentBuilder builder = new DocumentBuilder(doc);

            foreach (Node nd in insnotlast.nodecol) {                
                if (nd != insnotlast.nodecol[insnotlast.nodecol.Count - 1]) {
                    builder.MoveTo(nd);
                    builder.InsertBreak(BreakType.SectionBreakNewPage);
                }

                try {
                    nd.Remove();
                }
                catch { }
            }

            ReplaceDelEmptyPara rep = new ReplaceDelEmptyPara();
            rep = new ReplaceDelEmptyPara();
            doc.Range.Replace(new Regex(@"WF:DelEmptyPara", RegexOptions.IgnoreCase), rep, false);
            foreach (Node nd in rep.nodecol) {
                try {
                    nd.Remove();
                }
                catch { }
            }

            try {
               ArrayList sectList = new ArrayList();
                foreach (Section sect in doc.Sections) {
                    if (sect.Body.FirstParagraph != null) {
                        if (sect.Body.FirstParagraph.IsEndOfSection && string.IsNullOrEmpty(sect.Body.FirstParagraph.ToString().Trim())) {
                            sectList.Add(sect);
                        }
                    }
                }

                //remove empty sections
                foreach (Aspose.Words.Section sect in sectList) {
                    sect.Remove();
                }
            }
            catch {

            }


            try
            {
                // FORMATTING REPLACES
                // 1. In Field Bolding
                // 2. Nonbreaking spaces

                // 1. In Field Bolding
                FindMatchedNodes boldnodes = new FindMatchedNodes();
                doc.Range.Replace(new Regex(@"\[bold\].*?\[/bold\]", RegexOptions.IgnoreCase), boldnodes, true);
                foreach (Node nd in boldnodes.nodes)
                {
                    if (nd.GetType() == typeof(Run))
                    {
                        Run run = (Run)nd;
                        run.Font.Bold = true;
                    }
                }

                doc.Range.Replace(new Regex(@"\[bold\]", RegexOptions.IgnoreCase), "");
                doc.Range.Replace(new Regex(@"\[/bold\]", RegexOptions.IgnoreCase), "");

                // 1. In Field italics
                FindMatchedNodes italicnodes = new FindMatchedNodes();
                doc.Range.Replace(new Regex(@"\[italic\].*?\[/italic\]", RegexOptions.IgnoreCase), italicnodes, true);
                foreach (Node nd in italicnodes.nodes)
                {
                    if (nd.GetType() == typeof(Run))
                    {
                        Run run = (Run)nd;
                        run.Font.Italic = true;
                    }
                }


                doc.Range.Replace(new Regex(@"\[italic\]", RegexOptions.IgnoreCase), "");
                doc.Range.Replace(new Regex(@"\[/italic\]", RegexOptions.IgnoreCase), "");

                // 2. Non breaking spaces
                FindMatchedNodes nbspnodes = new FindMatchedNodes();
                doc.Range.Replace(new Regex(@"\[nbsp\]", RegexOptions.IgnoreCase), nbspnodes, true);
                foreach (Node nd in nbspnodes.nodes)
                {
                    builder.MoveTo(nd);
                    builder.Write(Aspose.Words.ControlChar.NonBreakingSpace);
                }
                doc.Range.Replace(new Regex(@"\[nbsp\]", RegexOptions.IgnoreCase), "");
            }
            catch (Exception ex)
            {
                ErrorAdd(ex);
            }

        }

        public void DocumentCleanDownJoinToDoc(ref Aspose.Words.Document doc) {

            WordFusion.Assembly.ReplaceSectionStartContinuous rep = new ReplaceSectionStartContinuous();
            doc.Range.Replace(new Regex(@"wf:SectionStart.Continuous", RegexOptions.IgnoreCase), rep, false);
            foreach (Node nd in rep.nodecol) {
                try {
                    nd.Remove();
                }
                catch { }
            }

            WordFusion.Assembly.ReplaceDelEmptyPara repafterjoin = new ReplaceDelEmptyPara();
            repafterjoin = new ReplaceDelEmptyPara();
            doc.Range.Replace(new Regex(@"WF:DelAfterJoin", RegexOptions.IgnoreCase), rep, false);
            foreach (Node nd in rep.nodecol) {
                try {
                    nd.Remove();
                }
                catch { }
            }

        }

        //// User_ should be changed to User: to be consistent with the User Logo and Reviewer data
        //private void UserInfoMerge(Document doc)
        //{
        //    try
        //    {
        //        DataTable usertbl = UserData.Tables["user"];
        //        if (usertbl != null)
        //        {
        //            if (usertbl.Rows.Count > 0)
        //            {
        //                foreach (DataColumn col in usertbl.Columns)
        //                {
        //                    col.ColumnName = "User_" + col.ColumnName;
        //                }
        //                doc.MailMerge.Execute(usertbl);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //ErrorAdd(ex);
        //        // Do nothing - something wrong with the USer Data
        //    }
        //}

        private void ReviewerInfoMerge(Document doc) {

            // Unused Method

            ////  should be passed in as a String
            //try {

            //    if (WordFusion.SystemFramework.Helpers.Types.isGuid(ReviewerID)) {

            //        DataSet ds = new User.User().ReviewerGet(ReviewerID);
            //        Aspose.Words.DocumentBuilder builder = new DocumentBuilder(doc);

            //        if (ds.Tables["user"].Rows.Count > 0) {
            //            DataRow row = ds.Tables["user"].Rows[0];
            //            Image logo = byteArrayToImage((byte[])row["Logo"]);
            //            if (logo != null) {
            //                while (builder.MoveToMergeField("Reviewer:Logo")) {
            //                    builder.InsertImage(logo);
            //                }
            //            }
            //            foreach (DataColumn col in ds.Tables["user"].Columns) {
            //                while (builder.MoveToMergeField("Reviewer:" +  col.ColumnName)) {
            //                    builder.Write(row[col.ColumnName].ToString());
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex) {
            //    // Do nothing - something wrong with the USer Data
            //}

        }

        private void SiteInfoMerge(Document doc) {

            //  should be passed in as a String
            try {

                if (SiteInfo != null) {

                    Aspose.Words.DocumentBuilder builder = new DocumentBuilder(doc);
                    if (SiteInfo.Tables["user"].Rows.Count > 0) {

                        DataRow row = SiteInfo.Tables["user"].Rows[0];

                        try {

                            if (row["Logo"] != System.DBNull.Value) {
                                Image logo = ByteArrayToImage((byte[])row["Logo"]);
                                if (logo != null) {
                                    using (MemoryStream ms = new MemoryStream()) {
                                        logo.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                                        Byte[] img = ms.ToArray();
                                        while (builder.MoveToMergeField("Site:Logo")) {
                                            builder.InsertImage(img);
                                        }
                                    }
                                }
                            }
                            else {
                                while (builder.MoveToMergeField("Site:Logo")) {
                                }
                            }
                        }
                        catch { }

                        foreach (DataColumn col in SiteInfo.Tables["user"].Columns) {
                            while (builder.MoveToMergeField("Site:" + col.ColumnName)) {
                                if ("Logo" != col.ColumnName) {
                                    if (row[col.ColumnName] != null) {
                                        builder.Write(row[col.ColumnName].ToString());
                                    }
                                }
                            }
                        }                        
                       
                    }
                }
                else {

                }
            }
            catch (Exception)
            {
                
                // Do nothing - something wrong with the USer Data
            }

        }

        public Image ByteArrayToImage(byte[] byteArrayIn) {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        private void LogoMerge(Document doc) {

        }

        public void WaterMarkApply(Document doc, String Text) {

            DocumentBuilder builder = new DocumentBuilder(doc);

            //Create whatermark shape
            Shape watermark = new Shape(doc, ShapeType.TextPlainText);
            watermark.TextPath.Text = Text;
            watermark.TextPath.FontFamily = "Times New Roman";
            watermark.Width = 360;
            watermark.Height = 360;
            watermark.Rotation = -45;
            //uncomment the following line if you need solid black text
            watermark.Fill.Color = Color.DarkGray;
            //Set position of watermark
            watermark.RelativeHorizontalPosition = RelativeHorizontalPosition.Page;
            watermark.RelativeVerticalPosition = RelativeVerticalPosition.Page;
            watermark.WrapType = WrapType.None;
            watermark.VerticalAlignment = VerticalAlignment.Center;
            watermark.HorizontalAlignment = HorizontalAlignment.Center;

            //Create new paragraph and append watermark to this paragraph
            Paragraph watermarkPar = new Paragraph(doc);
            watermarkPar.AppendChild(watermark);

            ArrayList hfTypes = new ArrayList
            {
                HeaderFooterType.HeaderPrimary,
                HeaderFooterType.HeaderFirst,
                HeaderFooterType.HeaderEven
            };

            //Now we should insert watermark into the header of each section of document
            foreach (Section sect in doc.Sections) {
                foreach (HeaderFooterType hftype in hfTypes) {
                    if (sect.HeadersFooters[hftype] == null) {
                        //Create new header
                        if (hftype == HeaderFooterType.HeaderPrimary ||
                            hftype == HeaderFooterType.HeaderFirst && sect.PageSetup.DifferentFirstPageHeaderFooter ||
                            hftype == HeaderFooterType.HeaderEven && sect.PageSetup.OddAndEvenPagesHeaderFooter) {
                            HeaderFooter hf = new HeaderFooter(doc, hftype);

                            //insert clone of watermarkPar into the header
                            hf.AppendChild(watermarkPar.Clone(true));
                            sect.HeadersFooters.Add(hf);
                        }
                    }
                    else {
                        //sect.HeadersFooters[hftype].AppendChild(watermarkPar.Clone(true));
                        Node nd = watermark.Clone(true);
                        sect.HeadersFooters[hftype].Paragraphs[0].AppendChild(nd);
                    }
                }
            }
        }

        public Image GetResourceContent(string filename) {
            Image retval = null;
            // get the current assembly
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            // resources are named using a fully qualified name
            System.IO.Stream strm = asm.GetManifestResourceStream(asm.GetName().Name + "." + filename);
            // read the contents of the embedded file

            Bitmap bmp = new Bitmap(strm);
            retval = bmp;
            return retval;
        }

        public String GetStringResourceContent(string filename) {
            String retval = null;
            // get the current assembly
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            // resources are named using a fully qualified name

            String assname = asm.GetName().Name;
            System.IO.Stream strm = asm.GetManifestResourceStream(assname + "." + filename);

            using (StreamReader reader = new StreamReader(strm)) {
                retval = reader.ReadToEnd();
            }

            //retval = strm.re;
            return retval;
        }
  
        private Boolean HasInsertFields(Aspose.Words.Document doc) {

            if (Inserts != null) {
                DocumentBuilder builder = new DocumentBuilder(doc);

                foreach (string str in Inserts) {
                    // Assemble the Insert Document from the boilerplate
                    char[] delimiterChars = { ',' };
                    string[] Parameters = str.Split(delimiterChars);
                    string Insert = Parameters[1];

                    if (HasMailMergeFields(doc)) {
                        string[] fldNames = doc.MailMerge.GetFieldNames();
                        foreach (String fldName in fldNames) {
                            if (Insert == fldName) return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool HasInsertHeadersFields(Document doc) {

            if (InsertsWithHeaders != null) {
                DocumentBuilder builder = new DocumentBuilder(doc);

                foreach (string str in InsertsWithHeaders) {
                    // Assemble the Insert Document from the boilerplate
                    char[] delimiterChars = { ',' };
                    string[] Parameters = str.Split(delimiterChars);
                    string Insert = Parameters[1];

                    if (HasMailMergeFields(doc)) {
                        string[] fldNames = doc.MailMerge.GetFieldNames();
                        foreach (String fldName in fldNames) {
                            if (Insert == fldName) return true;
                        }
                    }
                }
            }

            return false;
        }

       [System.Diagnostics.DebuggerNonUserCode()]
        private bool HasMailMergeFields(Document doc) {

            bool retval = false;
            try {

                string[] fldNames = doc.MailMerge.GetFieldNames();
                if (fldNames.Length > 0) retval = true;

            }
            catch {
                retval = false;
            }

            return retval;
        }

        /// <summary>
        /// Appends next paragraph nodes to the specified paragraph.
        /// Does not do anything if the specified paragraph is the last paragraph of the story or is followed by the table.
        /// </summary>
        private void PrevParagraphAppendTo(Paragraph para) {

            try {

                if (!(para.PreviousSibling is Paragraph)) {
                    // do nothing and return
                    return;
                }
                Paragraph prevPara = (Paragraph)para.PreviousSibling;

                int ndCount = para.ChildNodes.Count;
                for (int i = 0; i < ndCount - 1; i++) {
                    prevPara.AppendChild(para.ChildNodes[i]);
                }
            }
            catch (Exception) { }
            para.Remove();
        }



        private String DocHTMLTextCleanDown(String value) {

            String retval = value;
            value = value.Replace(">", "&gt;");
            value = value.Replace("<", "&lt;");
            value = value.Replace("«IF»", "<if>"); value = value.Replace("«If»", "<if>");
            value = value.Replace("«if»", "<if>"); value = value.Replace("«/IF»", "</if>");
            value = value.Replace("«/If»", "</if>"); value = value.Replace("«/if»", "</if>");

            return retval;

        }

        public Document FormatLists(Aspose.Words.Document doc) {

            try {

                DocumentBuilder builder = new DocumentBuilder(doc);
                foreach (Bookmark bkmark in doc.Range.Bookmarks) {

                    if (IsValidBookMark(bkmark)) {
                        if (bkmark.Text.Length > 11) {

                            // «ListFormat
                            if (bkmark.Text.Substring(0, 11).ToLower() == "«listformat") {
                                if (bkmark.Name.ToLower().Contains("listformat") || bkmark.Name.Substring(0, 3).ToLower() == "lf_") {

                                    String name = bkmark.Name;
                                    string[] stringSeparators = new string[] { "«ListItem" + name + "»" };
                                    string[] items = bkmark.Text.Split(stringSeparators, StringSplitOptions.None);
                                    String lstname = FormatListNameGet(bkmark.Text);

                                    for (int i = 0; i < items.Length - 1; i++) {
                                        String lstValue = FormatListValue(lstname, i, items.Length - 1);
                                        if (builder.MoveToMergeField("ListItem" + bkmark.Name, true, true)) {
                                            builder.Write(lstValue);
                                        };
                                    }
                                }
                            }
                        }
                    }
                }
                // Clean up the Statements
                doc.Range.Replace(new Regex(@"«ListItem»", RegexOptions.IgnoreCase), "");
                doc.Range.Replace(new Regex(@"«ListFormat.*?»", RegexOptions.IgnoreCase), "");
                doc.Range.Replace(new Regex(@"«/ListFormat»", RegexOptions.IgnoreCase), "");
            }
            catch (Exception ex) {
                ErrorAdd(ex);
            }

            return doc;
        }

        private Boolean IsValidBookMark(Bookmark bkmark) {
            Boolean retval = true;

            try {
                String s = bkmark.Text;
            }
            catch {
                retval = false;
            }

            return retval;
        }

        private String FormatListNameGet(String bkmark) {

            Regex regex = new Regex("«ListFormat" + ".*?»", RegexOptions.IgnoreCase);
            Match match = regex.Match(bkmark);
            String Switch = match.Value;

            char[] delimiterChars = { '[', ']' };
            string[] SwitchList = Switch.Split(delimiterChars);
            if (SwitchList.Length > 2) {
                String SwitchName = SwitchList[1];
                return SwitchName;
            }

            return "";

        }

        private string ConcatListGet(String ListType, String fldName, String Value, String Text) {

            try {

                Regex regex = new Regex("MERGEFIELD. ?" + fldName + ".*?«" + fldName + "»", RegexOptions.IgnoreCase);

                Match matField = regex.Match(Text);

                // Get The Field
                String mergeFld = matField.Value;
                int intStartField = Text.IndexOf(matField.Value);
                int intEndField = intStartField + mergeFld.Length;

                // Get The MergeText Values
                regex = new Regex(ListType + ".*?«" + fldName + "»", RegexOptions.IgnoreCase);
                Match matConcatFld = regex.Match(mergeFld);
                string ConcatFld = matConcatFld.Value;
                int intStartConcatFld = Text.IndexOf(matConcatFld.Value);
                int intEndConcatFld = intStartConcatFld + matConcatFld.Length;

                if (intStartConcatFld > intStartField && intEndConcatFld <= intEndField) {
                    char[] delimiterChars = { '(', ')' };
                    string[] ConcatList = ConcatFld.Split(delimiterChars);
                    if (ConcatList.Length > 2) {
                        String ConcatListName = ConcatList[1];
                        return ConcatListName;
                    }

                }
                return "";
            }
            catch (Exception ex) {
                ErrorAdd(ex);
            }
            return "";
        }

        private Document DocsClauseLibraryControlInsert(Collection<String> col, Document dstDoc) {

            foreach (string str in col) {
                // Assemble the Insert Document from the boilerplate

                char[] delimiterChars = { ';' };
                string[] Parameters = str.Split(delimiterChars);

                string fNameChild = Parameters[1];
                string InsLocation = Parameters[0];

                // Need to Assemble the Source Doc Also
                Document srcDoc = new Document(fNameChild); // = AssembledDocument(fNameChild);
                dstDoc = DocumentInsert(dstDoc, srcDoc, InsLocation);

            }

            // Need to find the Insert in the Document
            return dstDoc;
        }


        public enum InsertTypes { Insert, InsertWithHeader };

         /// <summary>
        /// Logged on User Credentials
        /// Used in Serverside document assembly to get Docs
        /// </summary>
        public String User {
            get;
            set;
        }

        private Document DocumentsAppend(Document dstDoc) {

            if (Appends != null ) {
                //m_Compiler.Inserts.Clear();                
                foreach (string str in Appends) {
                    // Assemble the Insert Document from the boilerplate

                    char[] delimiterChars = { ',' };
                    string[] Parameters = str.Split(delimiterChars);

                    Assembler assembler = new Assembler
                    {
                        UpdateFields = UpdateFields,
                        DataMergeText = DataMergeText,
                        EvaluateIF = this.EvaluateIF
                    };

                    string docid = Parameters[0];
                    docid = HttpUtility.UrlEncode(docid);

                    // If Online can't use local settings
                    Document srcdoc = null;

                    // Use web service call to server to get Insert Documents
                    MailMerge.SVC.Library.Library lib = new MailMerge.SVC.Library.Library
                    {
                        Timeout = 15000
                    };
                    docid = Encryption.Encrypt.Encrypt64(docid, Encryption.Keys.KeyWordFusionWebServices);

                    byte[] bydoc = lib.FileDocumentGet(docid, true, User);

                    if (bydoc != null) {
                        srcdoc = new Document(new MemoryStream(bydoc));
                    }

                    Document srcDoc = assembler.Assemble(srcdoc, Data, string.Empty);

                    foreach (Exception ex in assembler.Errors) {
                        Errors.Add(new Exception(ex.Message));
                    }

                    DocumentAppend2(dstDoc, srcDoc);

                }

            }

            return dstDoc;
        }

        private bool IsServerDoc(string docname) {
            Boolean retval = false;
            try {
                Guid ui = new Guid(docname);
                retval = true;
            }
            catch (Exception)
            {
                // Do nothing value is not a guid return false
            }
            return retval;
        }

        /// <summary>
        /// Copies all sections from one document to another.
        /// </summary>
        private Document DocumentInsert(Document dstDoc, Document srcDoc, string InsLocation) {

            //Create DocumentBuilder so we can modify the document.
            InsertDocumentAtBookamrk(InsLocation, dstDoc, srcDoc);

            return dstDoc;

        }

        private Document DocumentInsert(Document dstDoc, Document srcDoc, DocumentBuilder builder, string InsLocation) {

            // Create DocumentBuilder
            // DocumentBuilder builder = new DocumentBuilder(dstDoc);
            // Move cursor to bookmark and insert paragraph break

            builder.Writeln();
            //Content of srcdoc will be inserted after this node
            Node insertAfterNode = builder.CurrentParagraph.PreviousSibling;
            //Content of first paragraph of srcDoc will be apended to this parafraph
            Paragraph insertAfterParagraph = (Paragraph)insertAfterNode;
            //Content of last paragraph of srcDoc will be apended to this parafraph
            Paragraph insertBeforeParagraph = builder.CurrentParagraph;
            // We will be inserting into the parent of the destination paragraph.
            CompositeNode dstStory = insertAfterNode.ParentNode;
            // Loop through all sections in the source document.
            foreach (Section srcSection in srcDoc.Sections) {
                // Loop through all block level nodes (paragraphs and tables) in the body of the section.
                foreach (Node srcNode in srcSection.Body) {
                    // Do not insert node if it is a last empty paragarph in the section.
                    Paragraph para = srcNode as Paragraph;
                    if ((para != null) && para.IsEndOfSection && !para.HasChildNodes)
                        break;

                    //If current paragraph is first paragraph of srcDoc 
                    //then append its content to insertAfterParagraph
                    if (para.Equals(srcDoc.FirstSection.Body.FirstParagraph)) {
                        foreach (Node node in para.ChildNodes) {
                            Node dstNode = dstDoc.ImportNode(node, true, ImportFormatMode.KeepSourceFormatting);
                            insertAfterParagraph.AppendChild(dstNode);
                        }
                    }
                    //If current paragraph is last paragraph of srcDoc 
                    //then append its content to insertBeforeParagraph
                    else if (para.Equals(srcDoc.LastSection.Body.LastParagraph)) {
                        Node previouseNode = null;
                        foreach (Node node in para.ChildNodes) {
                            Node dstNode = dstDoc.ImportNode(node, true, ImportFormatMode.KeepSourceFormatting);
                            if (previouseNode == null)
                                insertBeforeParagraph.InsertBefore(dstNode, insertBeforeParagraph.FirstChild);
                            else
                                insertBeforeParagraph.InsertAfter(dstNode, previouseNode);
                            previouseNode = dstNode;
                        }
                    }
                    else {
                        // This creates a clone of the node, suitable for insertion into the destination document.
                        Node newNode = dstDoc.ImportNode(srcNode, true, ImportFormatMode.KeepSourceFormatting);

                        // Insert new node after the reference node.
                        dstStory.InsertAfter(newNode, insertAfterNode);
                        insertAfterNode = newNode;
                    }
                }
            }
            return dstDoc;
        }

        /// <summary>
        /// Inserts the document at bookamrk.
        /// </summary>
        /// <param name="bookmarkName">Name of the bookmark.</param>
        /// <param name="MainDoc">The main doc.</param>
        /// <param name="MargedDoc">The marged doc.</param>
        public void InsertDocumentAtBookamrk(string bookmarkName, Document MainDoc, Document MargedDoc) {

            // Create DocumentBuilder
            DocumentBuilder builder = new DocumentBuilder(MainDoc);
            if (builder.MoveToBookmark(bookmarkName)) {
                MainDoc.Range.Bookmarks[bookmarkName].Text = "";
            }

            builder.MoveToBookmark(bookmarkName, true, true);
            builder.Writeln();

            // Content of srcdoc will be inserted after this node
            Node insertAfterNode = builder.CurrentParagraph.PreviousSibling;

            // Content of first paragraph of srcDoc will be apended to this parafraph
            Paragraph insertAfterParagraph = (Paragraph)insertAfterNode;

            // Content of last paragraph of srcDoc will be apended to this parafraph
            Paragraph insertBeforeParagraph = builder.CurrentParagraph;

            // We will be inserting into the parent of the destination paragraph.
            CompositeNode dstStory = insertAfterNode.ParentNode;
            NodeImporter importer = new NodeImporter(MargedDoc, MainDoc, ImportFormatMode.KeepSourceFormatting);
            // Loop through all sections in the source document.
            foreach (Section srcSection in MargedDoc.Sections) {
                // Loop through all block level nodes (paragraphs and tables) in the body of the section.
                foreach (Node srcNode in srcSection.Body) {

                    // Do not insert node if it is a last empty paragarph in the section.
                    Paragraph para = srcNode as Paragraph;
                    // If current paragraph is first paragraph of srcDoc
                    // then appent its content to insertAfterParagraph
                    if ((para != null && (para.Equals(MargedDoc.FirstSection.Body.FirstChild)))) {
                        Paragraph dstParagraph = (Paragraph)importer.ImportNode(para, true);
                        while ((dstParagraph.HasChildNodes)) {
                            insertAfterParagraph.AppendChild(dstParagraph.FirstChild);
                        }

                        // If subdocument contains only one paragraph
                        // then copy content of insertBeforeParagraph to insertAfterParagraph
                        // and remove insertBeforeParagraph
                        if ((MargedDoc.FirstSection.Body.FirstParagraph.Equals(MargedDoc.LastSection.Body.FirstChild))) {
                            while (((insertBeforeParagraph.HasChildNodes))) {
                                insertAfterParagraph.AppendChild(insertBeforeParagraph.FirstChild);
                            }
                            insertBeforeParagraph.Remove();
                        }
                    }
                    //If current paragraph is last paragraph of srcDoc
                    //then appent its content to insertBeforeParagraph
                    else if ((para != null && (para.Equals(MargedDoc.LastSection.Body.LastChild)))) {

                        Node tofix = default(Node);
                        Node previouseNode = null;

                        foreach (Node node in para.ChildNodes) {
                            Node dstNode = importer.ImportNode(node, true);

                            if (((previouseNode == null))) {
                                tofix = insertBeforeParagraph.InsertBefore(dstNode, insertBeforeParagraph.FirstChild);
                            }
                            else {
                                tofix = insertBeforeParagraph.InsertAfter(dstNode, previouseNode);
                            }
                            previouseNode = dstNode;
                        }
                    }
                    else {

                        // This creates a clone of the node, suitable for insertion into the destination document.

                        Node newNode = importer.ImportNode(srcNode, true);
                        // Insert new node after the reference node.
                        dstStory.InsertAfter(newNode, insertAfterNode);
                        insertAfterNode = newNode;
                    }
                }
            }
        }

        public void InsertDocumentAtMergeField(DocumentBuilder builder, Document MainDoc, Document SourceDoc) {

            Node insertAfterNode = builder.CurrentParagraph.PreviousSibling ?? builder.CurrentParagraph;

            // Make sure that the node is either a paragraph or table.
            if ((!insertAfterNode.NodeType.Equals(NodeType.Paragraph)) &
              (!insertAfterNode.NodeType.Equals(NodeType.Table)))
                throw new ArgumentException("The destination node should be either a paragraph or table.");

            // We will be inserting into the parent of the destination paragraph.
            CompositeNode dstStory = insertAfterNode.ParentNode;

            // This object will be translating styles and lists during the import.
            NodeImporter importer = new NodeImporter(SourceDoc, insertAfterNode.Document, ImportFormatMode.UseDestinationStyles);

            // Loop through all sections in the source document.
            foreach (Section srcSection in SourceDoc.Sections) {
                // Loop through all block level nodes (paragraphs and tables) in the body of the section.
                foreach (Node srcNode in srcSection.Body) {
                    // Let's skip the node if it is a last empty paragraph in a section.
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

        public Document DocumentInsertHeaders(Document dstDoc, Document srcDoc, String InsLocation, Boolean InsertBreak) {

            try {

                //Create DocumentBuilder so we can modify the document.
                DocumentBuilder builder = new DocumentBuilder(dstDoc);

                if (builder.MoveToBookmark(InsLocation)) {
                    dstDoc.Range.Bookmarks[InsLocation].Text = "";
                }

                if (builder.MoveToBookmark(InsLocation)) {

                    //builder.CurrentSection.Remove();

                    builder.InsertBreak(BreakType.SectionBreakNewPage);

                    Section sectA = null;
                    sectA = builder.CurrentSection;

                    foreach (Section sectB in srcDoc.Sections) {

                        int sectBIndex = srcDoc.Sections.IndexOf(sectB);

                        // Content
                        sectA.PrependContent(sectB);

                        //Loop throught all Headers/Footers in the B document
                        foreach (HeaderFooter hfB in sectB.HeadersFooters) {
                            //Check whether current section from docA conteins
                            //Header/Footer with the same type as h/f from docB
                            if (sectA.HeadersFooters[hfB.HeaderFooterType] != null) {
                                //Append content from h/f B to h/f A
                                foreach (Node childB in hfB.ChildNodes) {
                                    //Import node
                                    Node childA = dstDoc.ImportNode(childB, true, ImportFormatMode.KeepSourceFormatting);
                                    //Appent node to h/f
                                    sectA.HeadersFooters[hfB.HeaderFooterType].AppendChild(childA);
                                }
                            }
                            else {

                                //Copy whole h/f
                                Node hfA = dstDoc.ImportNode(hfB, true, ImportFormatMode.KeepSourceFormatting);
                                //Insert h/f
                                sectA.HeadersFooters.Add(hfA);
                            }
                        }
                    }
                }

            }
            catch (Exception ex) {
                ErrorAdd(ex);
            }
            return dstDoc;
        }

        public Document DocumentAppend2(Document dstDoc, Document srcDoc) {

            try {

                if (dstDoc.Range.Text == "" || dstDoc.Range.Text == "\f") dstDoc.RemoveAllChildren();

                MemoryStream stream = new MemoryStream();
                srcDoc.Save(stream,  SaveFormat.Doc );
                srcDoc = new Document(stream);
                
                if (dstDoc.FirstSection != null) {
                    dstDoc.FirstSection.PageSetup.SectionStart = SectionStart.NewPage;
                }
                
                dstDoc.AppendDocument(srcDoc, ImportFormatMode.KeepSourceFormatting);

            }
            catch (Exception ex) {
                ErrorAdd(ex);
            }
            return dstDoc;
        }

        public void DocumentAppend(Document dstDoc, Document srcDoc) {

            DocumentBuilder builder = new DocumentBuilder(dstDoc);
            builder.MoveToDocumentEnd();
            builder.InsertBreak(BreakType.SectionBreakNewPage);

            builder.CurrentSection.HeadersFooters.LinkToPrevious(false);

            foreach (Section srcSection in srcDoc.Sections) {

                //Import section
                Section newDstSection = (Section)dstDoc.ImportNode(srcSection, true, ImportFormatMode.KeepSourceFormatting);

                if (srcSection.Equals(srcDoc.FirstSection)) {

                    //Clone first paragraph of source document
                    Paragraph firstParagraph = (Paragraph)newDstSection.Body.FirstParagraph.Clone(true);

                    //Remove first paragraph from section
                    newDstSection.Body.FirstParagraph.Remove();

                    //Get last paragraph of destination document
                    Paragraph lastParagraph = dstDoc.LastSection.Body.LastParagraph;

                    //Append content of first section of src document to last section of dst document
                    dstDoc.LastSection.AppendContent(newDstSection);

                    //Copy text from first paragraph of srcDoc to last paragraph of dstDoc
                    foreach (Node node in firstParagraph.ChildNodes) {
                        lastParagraph.AppendChild(node);
                    }
                }
                else {
                    //Add section
                    dstDoc.AppendChild(newDstSection);
                }
            }
        }

        public Document DocumentAppend3(Document dstDoc, Document srcDoc) {

            try {

                //Create DocumentBuilder so we can modify the document.
                DocumentBuilder builder = new DocumentBuilder(dstDoc);
                builder.MoveToDocumentEnd();
               
                builder.MoveToDocumentEnd();
                Section sectA = builder.CurrentSection;
                // Unlink header and footers.
                sectA.HeadersFooters.LinkToPrevious(false);

                foreach (Section sectB in srcDoc.Sections) {
                    // Import whole section into the destination document.
                    Node dstSection = dstDoc.ImportNode(sectB, true, ImportFormatMode.UseDestinationStyles);

                    // Insert section.
                    sectA.ParentNode.InsertAfter(dstSection, sectA);
                }


            }
            catch (Exception ex) {
                ErrorAdd(ex);
            }
            return dstDoc;
        }

        private void NodesImport(Document doc, DocumentBuilder builder, CompositeNode parentNode) {
            foreach (Node nd in parentNode.ChildNodes) {
                if (nd.IsComposite) {
                    NodesImport(doc, builder, (CompositeNode)nd);
                }
                else {
                    Node impNode = (Node)doc.ImportNode(nd, true, ImportFormatMode.KeepSourceFormatting);
                    builder.InsertNode(impNode);
                }
            }
        }

        # endregion Assemble

        # region "Formatted Lists"

        //  make this generic for all switches
        private string FormatListNameGet(String fldName, String Value, String Text) {

            try {

                //«Attorney_Address.*?»
                Regex regex = new Regex("MERGEFIELD. ?" + fldName + ".*?«" + fldName + "»", RegexOptions.IgnoreCase);
                Match matField = regex.Match(Text);

                if (matField.Value == "") {
                    regex = new Regex("«" + fldName + ".*?»", RegexOptions.IgnoreCase);
                    matField = regex.Match(Text);
                }

                // Get The Field
                String fld = matField.Value;
                int intFldStart = Text.IndexOf(matField.Value);
                int intFldEnd = intFldStart + fld.Length;

                // Get The MergeText Values
                //regex = new Regex("ListFormat" + ".*?«" + fldName + "»", RegexOptions.IgnoreCase);
                regex = new Regex("ListFormat" + ".*?»", RegexOptions.IgnoreCase);
                Match matFld = regex.Match(fld);
                string Switch = matFld.Value;
                int intSwitchStart = Text.IndexOf(matFld.Value);
                int intSwitchEnd = intSwitchStart + matFld.Length;

                if (intSwitchStart > intFldStart && intSwitchEnd <= intFldEnd) {
                    char[] delimiterChars = { '[', ']' };
                    string[] SwitchList = Switch.Split(delimiterChars);
                    if (SwitchList.Length > 2) {
                        String SwitchName = SwitchList[1];
                        return SwitchName;
                    }
                }
                return "";
            }
            catch (Exception ex) {
                ErrorAdd(ex);
            }
            return "";
        }

        public string FormatListValue(String ListFormat, int rowindex, int rowcount) {

            switch (ListFormat) {
                case "a":
                    return " ";
                case "a and b":
                    return FormatType1Get(" and", rowindex, rowcount);
                case "a, b and c":
                    return FormatType2Get(" and", ",", rowindex, rowcount);
                case "a, b and c.":
                    return FormatType5Get(" and", ",", ".", rowindex, rowcount);
                case "a; b; and c.":
                    return FormatType4Get(" and", ";", ".", rowindex, rowcount);
                case "a; b; and c,":
                    return FormatType4Get(" and", ";", ",", rowindex, rowcount);
                case "a, b, and c.":
                    return FormatType4Get(" and", ";", ".", rowindex, rowcount);
                case "a; b; and c;":
                    return FormatType4Get(" and", ";", ";", rowindex, rowcount);

                case "a; b; ; and c;":
                    return FormatType4Get(" and", ";", ";", rowindex, rowcount);
                case "a; b; ; and c.":
                    return FormatType4Get(" and", ";", ".", rowindex, rowcount);
                case "a; b; ; and c,":
                    return FormatType4Get(" and", ";", ",", rowindex, rowcount);

                case "a; b; ;and c;":
                    return FormatType4Get(" and", ";", ";", rowindex, rowcount);
                case "a; b; ;and c.":
                    return FormatType4Get(" and", ";", ".", rowindex, rowcount);
                case "a; b; ;and c,":
                    return FormatType4Get(" and", ";", ",", rowindex, rowcount);

                case "a; b and c":
                    return FormatType2Get(" and", ";", rowindex, rowcount);
                case "a; and b; and c.":
                    return FormatType3Get(" and", "; and ", ".", rowindex, rowcount);
                case "a or b":
                    return FormatType1Get(" or ", rowindex, rowcount);
                case "a, b or c":
                    return FormatType2Get(" or ", ",", rowindex, rowcount);
                case "a; b or c":
                    return FormatType2Get(" or ", ";", rowindex, rowcount);
            }

            return "";
        }

        private String FormatType1Get(String type, int rowindex, int rowcount) {

            if (rowindex == 0 && rowcount == 1) return " ";
            if (rowindex == 0 && rowcount == 2) return type;
            if (rowindex == 0 && rowcount > 2) return type;
            if (rowindex > 0 && rowindex <= rowcount - 2) return type;
            if (rowindex > 0 && rowindex == rowcount - 1) return "";

            return "";

        }

        private String FormatType2Get(String type, String delim, int rowindex, int rowcount) {

            if (rowindex == 0 && rowcount == 1) return " ";
            if (rowindex == 0 && rowcount == 2) return type;
            if (rowindex == 0 && rowcount > 2) return delim + " ";

            if (rowindex > 0 && rowindex == rowcount - 2) return type;
            if (rowindex > 0 && rowindex <= rowcount - 2) return delim + " ";
            if (rowindex > 0 && rowindex == rowcount - 1) return "";

            return "";

        }

        private String FormatType3Get(String type, String delim, String term, int rowindex, int rowcount) {

            String retval = "";

            if (rowindex == 0 && rowcount == 1) retval = term;
            if (rowindex == 0 && rowcount == 2) retval = type;
            if (rowindex == 0 && rowcount > 2) retval = delim;

            if (rowindex > 0 && rowindex <= rowcount - 2) retval = delim;
            if (rowindex > 0 && rowindex == rowcount - 1) retval = term;

            return retval;

        }


        private String FormatType4Get(String type, String delim, String term, int rowindex, int rowcount) {

            String retval = "";

            if (rowindex == 0 && rowcount == 1) retval = term;
            if (rowindex == 0 && rowcount == 2) retval = type;
            if (rowindex == 0 && rowcount > 2 || rowindex > 0 && rowindex <= rowcount - 2) retval = delim;

            if (rowindex > 0 && rowindex == rowcount - 2) retval = delim + type;
            if (rowindex > 0 && rowindex == rowcount - 1) retval = term;

            return retval;

        }

        private String FormatType5Get(String type, String delim, String term, int rowindex, int rowcount) {

            String retval = "";

            if (rowindex == 0 && rowcount == 1) retval = term;
            if (rowindex == 0 && rowcount == 2) retval = type;
            if (rowindex == 0 && rowcount > 2) retval = delim;

            if (rowindex > 0 && rowindex <= rowcount - 2) retval = type;
            if (rowindex > 0 && rowindex == rowcount - 1) retval = term;

            return retval;

        }

        # endregion

        #region "Bookmarks"

        private readonly ArrayList BookmarkNodes;
        private bool IsBookmarkNode(Node node) {
            return (node.NodeType == NodeType.BookmarkStart) || (node.NodeType == NodeType.BookmarkEnd);
        }

        private bool StoreIfBookmark(Hashtable bookmarkNames, Hashtable bookmarkStarts, Hashtable bookmarkEnds, Node node) {
            try {
                if (node.NodeType == NodeType.BookmarkStart) {
                    BookmarkStart bookmarkStart = (BookmarkStart)node;
                    bookmarkNames[bookmarkStart.Name] = null;
                    bookmarkStarts.Add(bookmarkStart.Name, bookmarkStart);
                    return true;
                }
                else if (node.NodeType == NodeType.BookmarkEnd) {
                    BookmarkEnd bookmarkEnd = (BookmarkEnd)node;
                    bookmarkNames[bookmarkEnd.Name] = null;
                    bookmarkEnds.Add(bookmarkEnd.Name, bookmarkEnd);
                    return true;
                }
            }
            catch (Exception)
            {

            }

            return false;
        }

        private Node RemoveBookmarkNode(string name, Hashtable bookmarkNodes) {
            Node node = (Node)bookmarkNodes[name];
            node.Remove();
            bookmarkNodes.Remove(name);
            return node;
        }

        private void MoveBookmarkNode(string name, Hashtable bookmarkNodes, Paragraph para) {
            para.PrependChild(RemoveBookmarkNode(name, bookmarkNodes));
        }

        private void DeleteBetweenBookmarks(Document doc, Bookmark bk) {

            try {

                bk.Text = "";
                bk.BookmarkStart.Remove();

            }
            catch (Exception)
            {
                // Do nothing as the bookmark does not exist
                //Errors.Add(ex.InnerException);
            }

        }


        private void ColourBookmark(Document doc, Bookmark bkmark, Color col) {

            for (Node node = bkmark.BookmarkStart; (node != bkmark.BookmarkEnd) && (node != null); node = node.NextPreOrder(doc)) {
                if (node.NodeType == NodeType.Run) {
                    Run run = (Run)node;
                    run.Font.Bold = true;
                    run.Font.HighlightColor = col;
                }
            }
        }

        #endregion

        #endregion

        #region Load Save

        ///// <summary>
        ///// Gets the output file name that consists of the selected demo name and selected output format.
        ///// </summary>
        //private string FileNameGetProposed(WordFusion.SystemFramework.FileManager.DocumentTypes docType) {

        //    string fileExt = FileExtGet(docType);

        //    if (SaveFileName == "" || SaveFileName == null) {
        //        FileInfo fInfoAnswer = new FileInfo(AnswerfName);
        //        FileInfo fInfoTemplate = new FileInfo(fNameTemplate);

        //        string[] fNameAnswer = fInfoAnswer.Name.Split('.');
        //        string[] fNameTmplt = fInfoTemplate.Name.Split('.');

        //        string fName = fNameAnswer[0] + "_" + fNameTmplt[0];

        //        return Path.Combine(SaveFolder, string.Format("{0}.{1}", fName, fileExt));
        //    }

        //    return Path.Combine(SaveFolder, SaveFileName);

        //}

        /// <summary>
        /// Gets a valid File Extentions for this type of Document
        /// </summary>
        public string FileExtGet(WordFusion.SystemFramework.FileManager.DocumentTypes docType) {
            switch (docType) {
                case WordFusion.SystemFramework.FileManager.DocumentTypes.Word2007:
                    return "docx";
                case WordFusion.SystemFramework.FileManager.DocumentTypes.Word2003:
                    return "doc";
                case WordFusion.SystemFramework.FileManager.DocumentTypes.PDF:
                    return "pdf";
                case WordFusion.SystemFramework.FileManager.DocumentTypes.RTF:
                    return "rtf";
                case WordFusion.SystemFramework.FileManager.DocumentTypes.HTML:
                    return "htm";
                default:
                    throw new ApplicationException("Unknown output format.");
            }
        }

        # endregion

        # region General

        /// <summary>
        /// Adds Error record to the errors collection
        /// </summary>
        private void ErrorAdd(Exception ex) {
            Errors.Add(ex);
        }

        # endregion

    }

    public struct BatchAssembleItem {

        private string m_fNameAFile;
        public string FNameAFile {
            get { return m_fNameAFile; }
            set { m_fNameAFile = value; }
        }

        private string m_fNameQFile;
        public string FNameQFile {
            get { return m_fNameQFile; }
            set { m_fNameQFile = value; }
        }

        private string m_fNameDFile;
        public string FNameDFile {
            get { return m_fNameDFile; }
            set { m_fNameDFile = value; }
        }
    }

    public class FindMatchedNodes : IReplacingCallback {

        //Store Matched nodes in ArrayList
        public ArrayList nodes = new ArrayList();

        ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e) {
            // This is a Run node that contains either the beginning or the complete match.
            Node currentNode = e.MatchNode;

            // The first (and may be the only) run can contain text before the match,
            // in this case it is necessary to split the run.
            if (e.MatchOffset > 0)
                currentNode = SplitRun((Run)currentNode, e.MatchOffset);

            ArrayList runs = new ArrayList();

            // Find all runs that contain parts of the match string.
            int remainingLength = e.Match.Value.Length;
            while (

                (remainingLength > 0) &&

                (currentNode != null) &&

                (currentNode.GetText().Length <= remainingLength)) {

                runs.Add(currentNode);

                remainingLength = remainingLength - currentNode.GetText().Length;

                // Select the next Run node.
                // Have to loop because there could be other nodes such as BookmarkStart etc.
                do {
                    currentNode = currentNode.NextSibling;
                }
                while ((currentNode != null) && (currentNode.NodeType != NodeType.Run));
            }

            // Split the last run that contains the match if there is any text left.
            if ((currentNode != null) && (remainingLength > 0)) {
                SplitRun((Run)currentNode, remainingLength);
                runs.Add(currentNode);
            }

            string runText = "";
            foreach (Run run in runs)
                runText += run.Text;

            ((Run)runs[0]).Text = runText;

            for (int i = 1; i < runs.Count; i++) {
                ((Run)runs[i]).Remove();
            }

            nodes.Add(runs[0]);

            // Signal to the replace engine to do nothing because we have already done all what we wanted.
            return ReplaceAction.Skip;
        }
        
        /// <summary>
        /// Splits text of the specified run into two runs.
        /// Inserts the new run just after the specified run.
        /// </summary>
        private static Run SplitRun(Run run, int position) {
            Run afterRun = (Run)run.Clone(true);
            afterRun.Text = run.Text.Substring(position);
            run.Text = run.Text.Substring(0, position);
            run.ParentNode.InsertAfter(afterRun, run);
            return afterRun;
        }

    }

}