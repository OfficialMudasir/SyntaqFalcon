using System;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using Aspose.Words;
 
namespace WordFusion.Assembly.MailMerge
{
    public delegate void InsertHandlerEvent(Aspose.Words.Document document, MergeField mergeField, string switchValue);

    internal class MergeFieldFormatter
    {

        public static event InsertHandlerEvent InsertHandler;
        public static Aspose.Words.Document document;

        public static string FormatValue(MergeField mergeField, string value, DataSet data, List<MergeRegionData> regiondata, DataRow row)
        {
            foreach (MergeFieldSwitch fieldSwitch in mergeField.Switches){
                value = MergeFieldFormatter.FormatValue(mergeField, fieldSwitch.Name, fieldSwitch.Value, value, data, regiondata,row);
            }

            return value;
        }

        public static string FormatValue(MergeField mergeField, string switchType, string switchValue, string value, DataSet data, List<MergeRegionData> regiondata, DataRow row){

            switch (switchType.ToLower()){
                
                case "*":
                    value = MergeFieldFormatter.ProcessFormatSwitch(switchValue, value);
                    break;
                case "#":
                    value = MergeFieldFormatter.ProcessNumericSwitch(switchValue, value);
                    break;
                case "@":
                    value = MergeFieldFormatter.ProcessDateTimeSwitch(switchValue, value);
                    break;
                case "abs":
                    double num;
                    if (double.TryParse(value, out num)) {
                        value = Math.Abs(num).ToString();
                    }
                    break;
                case "b":
                    value = MergeFieldFormatter.ProcessTextBefore(switchValue, value);
                    break;
                case "buildlist":
                    value = MergeFieldFormatter.ProcessBuildList(mergeField, switchValue, value, data);
                    break;
                case "defaultifempty":
                    value = MergeFieldFormatter.ProcessDefaultIfEmpty(mergeField, switchValue, value);
                    break;
                case "delimtedlist":
                    value = MergeFieldFormatter.ProcessDelimitedList(mergeField, switchValue, value);
                    break;
                case "bulletarrowhead":
                case "bulletasterik":
                case "bulletcircle":
                case "bulletdefault":
                case "bulletdiamonds":
                case "bulletdisk":
                case "bulletsquare":
                case "bullettick":
                case "bulletasterisk":
                case "numberarabicdot":
                case "numberarabicparenthesis":
                case "numberdefault":
                case "numberlowercaseletterdot":
                case "numberlowercaseletterparenthesis":
                case "numberlowercaseromandot":
                case "numberuppercaseletterdot":
                case "numberuppercaseromandot":
                case "outlinebullets":
                case "outlineheadingsarticlesection":
                case "outlineheadingschapter":
                case "outlineheadingslegal":
                case "outlineheadingsnumbers":
                case "outlinelegal":
                case "outlinenumbers":
                    value = MergeFieldFormatter.ProcessBullet(mergeField, switchType, value);
                    break;
                case "possessive":
                    value = Possessive(value);
                    break;
                case "dollartext":
                    value = NumberToText.changeCurrencyToWords(value).Trim();
                    break;
                case "f":
                    value = MergeFieldFormatter.ProcessTextAfter(switchValue, value);
                    break;
                case "insert":
                    //InsertHandler(null, mergeField, switchValue);
                    break;
                case "lower":
                    value = value.ToLower();
                    break;
                case "mask":
                    value = MaskValue(switchValue, value);
                    break;
                case "mtext": case "mtextlist": case "subtext": case "sub":
                    value = MergeFieldFormatter.ProcessMergeText(switchValue, mergeField.Name, regiondata, row);
                    break;
                case "numbertext":
                    value = NumberToText.changeNumericToWords(value).Trim();
                    break;
                case "stringformat":
                    value = MergeFieldFormatter.ProcessStringFormat(switchValue, value);
                    break;
                case "upper":
                    value = value.ToUpper();
                    break;
                case "round":
                    double numround;
                    if (double.TryParse(value, out numround)) {
                        if (int.TryParse(switchValue, out int switchvaluenum))
                        {
                            value = Math.Round(numround, switchvaluenum).ToString();
                        }
                    }
                    break;
                case "substitute":
                    value = switchValue;
                    break;
            }

            return value;
        }

        private static string ProcessBuildList(MergeField mergeField, string switchvalue, string value, DataSet data){

            var rpts = switchvalue.Split('[', ']');
            var listvalue = string.Empty;

            System.Collections.ObjectModel.Collection<string> stringCol = new System.Collections.ObjectModel.Collection<string>();

            foreach (string rpt in rpts){

                try{
                    if (! string.IsNullOrEmpty(rpt)){
                        var items = rpt.Split(',', '|');
                        if(items.Length > 1){
                            var rptname = items[0].Trim(' ');
                            var fldname = items[1].Trim(' ');
                            if (data.Tables.Contains(rptname)){
                                DataTable tbl = data.Tables[rptname];
                                if (tbl.Columns.Contains(fldname)){
                                    var filter = items.Length > 2 ? items[2] : "";
                                    var datarows = tbl.Select(filter);
                                    foreach (DataRow row in datarows){
                                        stringCol.Add(row[fldname].ToString());
                                        //listvalue += row[fldname] + ", ";
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }

            }

            try
            {
                foreach (string item in stringCol)
                {

                    string delim = ", ";

                    int indexof = stringCol.IndexOf(item);
                    if (indexof == stringCol.Count - 2)
                    {
                        delim = " and ";
                    }
                    if (indexof == stringCol.Count - 1)
                    {
                        delim = "";
                    }

                    value += item + delim;
                }
            }
            catch { }

            //try{
            //    var lastParenSet = listvalue.LastIndexOf(", ");
            //    if(lastParenSet > 0){
            //        value = listvalue.Substring(0, lastParenSet > -1 ? lastParenSet : listvalue.Count());
            //        lastParenSet = value.LastIndexOf(", ");
            //        value = value.Remove(lastParenSet, 2).Insert(lastParenSet, " and ");
            //    }
            //}
            //catch { }

            return value;
        }

        private static string ProcessDefaultIfEmpty(MergeField mergeField, string switchvalue, string value)
        {

            //DocumentBuilder builder = new DocumentBuilder(document);
            //builder.MoveTo(mergeField.Start);
            var retval = value;

            if (string.IsNullOrEmpty(value))
            {
                retval = switchvalue;
            }

            return retval;

        }

        private static string ProcessDelimitedList(MergeField mergeField, string switchvalue, string value) {

            //DocumentBuilder builder = new DocumentBuilder(document);
            //builder.MoveTo(mergeField.Start);
            var retval = string.Empty;

            switchvalue = string.IsNullOrEmpty(switchvalue) ? "," : switchvalue;

            string[] values = value.Trim(',').Split(',');
            foreach (string itm in values) {
                if (!string.IsNullOrEmpty(itm)) {
                    retval += itm  +  switchvalue + " ";
                }
            }

            retval = retval.TrimEnd(' ');
            retval = retval.TrimEnd(switchvalue.ToCharArray());
            return retval;

        }

        private static string ProcessBullet(MergeField mergeField, string switchType, string value) {

            DocumentBuilder builder = new DocumentBuilder(document);
            builder.MoveTo(mergeField.Start);

            string asterisk = string.Empty;

            switch (switchType.ToLower()) {
                case "bulletasterik":
                    asterisk = "* ";
                    builder.ListFormat.List = null;
                    break;
                case "bulletarrowhead":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.BulletArrowHead);
                    break;
                case "bulletcircle":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.BulletCircle);
                    break;
                case "bulletdefault":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.BulletDefault);
                    break;
                case "bulletdiamonds":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.BulletDiamonds);
                    break;
                case "bulletdisk":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.BulletDisk);
                    break;
                case "bulletsquare":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.BulletSquare);
                    break;
                case "bullettick":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.BulletTick);
                    break;
                case "numberarabicdot":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.NumberArabicDot);
                    break;
                case "numberarabicparenthesis":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.NumberArabicParenthesis);
                    break;
                case "numberdefault":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.NumberDefault);
                    break;
                case "numberlowercaseletterdot":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.NumberLowercaseLetterDot);
                    break;
                case "numberlowercaseletterparenthesis":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.NumberLowercaseLetterParenthesis);
                    break;
                case "numberlowercaseromandot":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.NumberLowercaseRomanDot);
                    break;
                case "numberuppercaseletterdot":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.NumberUppercaseLetterDot);
                    break;
                case "numberuppercaseromandot":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.NumberUppercaseRomanDot);
                    break;
                case "outlinebullets":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.OutlineBullets);
                    break;
                case "outlineheadingsarticlesection":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.OutlineHeadingsArticleSection);
                    break;
                case "outlineheadingschapter":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.OutlineHeadingsChapter);
                    break;
                case "outlineheadingslegal":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.OutlineHeadingsLegal);
                    break;
                case "outlineheadingsnumbers":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.OutlineHeadingsNumbers);
                    break;
                case "outlinelegal":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.OutlineLegal);
                    break;
                case "outlinenumbers":
                    builder.ListFormat.List = document.Lists.Add(Aspose.Words.Lists.ListTemplate.OutlineNumbers);
                    break;
            }

            if (mergeField.Switches.Any(i => i.Name.ToLower() == "indent")) {

                int indent = 999;
                int.TryParse(mergeField.Switches.First(i => i.Name.ToLower() == "indent").Value, out indent);
                if(indent != 999) builder.ParagraphFormat.LeftIndent = indent;
            }

            string[] values = value.Trim(',').Split(',');
            foreach (string itm in values) {
                if(!string.IsNullOrEmpty(itm)) {
                    builder.Writeln(asterisk + itm);
                }
            }

            builder.ListFormat.RemoveNumbers();

            return string.Empty;
        }

        private static string MaskValue(string mask, string value) {

            try {
                var builder = new System.Text.StringBuilder();
                var maskIndex = 0;
                var valueIndex = 0;
                while (maskIndex < mask.Length && valueIndex < value.Length) {

                    if (mask[maskIndex] == '-') {
                        builder.Append('-');

                        if (value[maskIndex] == '-' || value[maskIndex] == '-') {
                            valueIndex++;
                        }

                        maskIndex++;

                    }
                    else if (mask[maskIndex] == ' ') {
                        builder.Append(' ');

                        if (value[maskIndex] == ' ' || value[maskIndex] == '-') {
                            valueIndex++;
                        }

                        maskIndex++;

                    }
                    else {
                        builder.Append(value[valueIndex]);
                        maskIndex++;
                        valueIndex++;
                    }

                }

                // Add in the remainder of the value
                if (valueIndex + 1 < value.Length) {
                    builder.Append(value.Substring(valueIndex));
                }

                return builder.ToString();
            }
            catch {
                return value;
            }

        }

        private static string Possessive(string value) {

            if (value.Length > 0) {

                var lastchar = value.ToLower()[value.Length - 1];

                if(lastchar == 's') {
                    value += "'";
                }
                else {
                    value += "'s";
                }
            }

            return value;

        }

        private static string ProcessFormatSwitch(string switchValue, string value){
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            switch (switchValue.ToLower()){
                case "caps":
                    value = textInfo.ToTitleCase(value);
                    break;

                case "firstcap":
                    value = MergeFieldFormatter.UppercaseFirst(value);
                    break;

                case "upper":
                    value = textInfo.ToUpper(value);
                    break;

                case "lower":
                    value = textInfo.ToLower(value);
                    break;

                case "alphabetic":
                    // :
                    break;

                case "arabic":
                    // :
                    break;

                case "cardtext":
                    // :
                    break;
                case "numbertext":
                    value = NumberToText.changeNumericToWords(value).Trim();
                    break;
                case "dollartext":
                    value =  NumberToText.changeCurrencyToWords(value).Trim();
                    break;
                case "hex":
                    // :
                    break;

                case "ordtext":
                    // :
                    break;

                case "ordinal":
                    // :
                    break;

                case "roman":
                    // :
                    break;

                case "charformat":
                    // :
                    break;

                case "mergeformat":
                    // :
                    break;
            }

            return value;
        }

        private static string ProcessStringFormat(String switchvalue, String value) {
            
            String retval = value;

            // i.e
            //retval = String.Format("{0} Sister and {0} Mother.", "your")
            char[] delimiterChars = { ',' };
            String[] args = switchvalue.Split(delimiterChars);

            //if (! value.Contains("{0}")) {
            //    value = "{0}" + value;
            //}
            foreach(string arg in args){
                retval = String.Format(value, arg);
            }
            

            return retval;
        }

        private static string ProcessMergeText(String lstname, String fldName,  List<MergeRegionData> regiondata, DataRow row) {

            String retval;

            // Remove talking marks
            DataSet data =  new DataSet() ;
            foreach (MergeRegionData tbl in regiondata){
                data.Merge(tbl.Data );
            }

            retval =  MergeTextValueGet( lstname, fldName, data, row);
            return retval;
        }

        private static string MergeTextValueGet(String lstname, String fldName, DataSet regiondata, DataRow row) {

            lstname = lstname.Replace("(", "");
            lstname = lstname.Replace(")", "");

            fldName = fldName.Trim();

            string selectedindex = row.Table.Columns.Contains(fldName + "_SelectedIndex") ?  row[fldName + "_SelectedIndex"].ToString(): "0";
            string selectedvalue = row.Table.Columns.Contains(fldName) ? row[fldName].ToString() : "";

                if (row != null) {
                     
                    if (regiondata.Tables[lstname] == null) return "ERROR: " + lstname + " MergeText List does not exist.Field: " + fldName ; //; throw new Exception(lstname + " MergeText List does not exist. Field: " + fldName);
                    if (regiondata.Tables[lstname] != null) {

                        DataTable dtMText = regiondata.Tables[lstname];

                        // Get the DefaultViewManager of a DataTable.
                        DataView dv = dtMText.DefaultView;

                        dv.Sort = "Order ASC";
                        Int32 index = Convert.ToInt32(selectedindex);

                        // None Selected
                        if (index == -1) return null;

                        if (index > dtMText.Rows.Count - 1) {
                            //Exception ex = new Exception(lstname);
                            //throw new IndexOutOfRangeException(index.ToString(), ex);
                            return string.Empty;
                        }
                        else {
                        
                                DataRow rowMText = null;
                                if(dtMText.Select("key = '" + selectedvalue + "'").Any())
                                {
                                    rowMText = dtMText.Select("key = '" + selectedvalue + "'").FirstOrDefault();
                                }
                                else
                                {
                                    if (dtMText.Rows[Convert.ToInt32(selectedindex)] != null)
                                    {
                                        rowMText = dtMText.Rows[Convert.ToInt32(selectedindex)];
                                    }
                                }

                                if(rowMText != null){

                                        if (rowMText["Value"] != null) {
                                            String SelectedValue = rowMText["Value"].ToString();

                                            // does this mergetext value lookup row data?
                                            string[] selectedfields = SelectedValue.Split('{', '{', '}', '}');

                                            for (int n = 0; n < selectedfields.Length - 1; n++) {
                                                if (row.Table.Columns[selectedfields[n]] != null) {
                                                    if (row[selectedfields[n]] != null) {
                                                        selectedfields[n] = (string)row[selectedfields[n]];
                                                    }
                                                }
                                            }

                                            StringBuilder builder = new StringBuilder();
                                            foreach (string value in selectedfields) {
                                                builder.Append(value);
                                            }

                                            // Iterate Nested MergeText Lists
                                            // i.e Mrs - MText(yesno, Value2)
                                            Regex reg = new Regex("MText(.*)", RegexOptions.IgnoreCase);
                                            MatchCollection results = reg.Matches(builder.ToString());
                                            foreach (Match match in results) {
                                                // Match result = (yesno, Value2)

                                                string[] mtext = match.Value.Split(new string[] { "MTEXT(", "MText(", ",", ")" }, StringSplitOptions.RemoveEmptyEntries);
                                                if (mtext.Length == 2) {
                                                    string nestedmtext = MergeTextValueGet(mtext[0], mtext[1], regiondata, row);
                                                    builder = new StringBuilder(reg.Replace(builder.ToString(), nestedmtext, 1));
                                                }
                                            }

                                            return builder.ToString();
                                        }
                                }


                            //}
                        }

   
                    }

            }

            return "ERROR:  MergeText List " + fldName + " not found in data";
        }

        private static string ProcessTextBefore(string switchValue, string value) {
            
            // Remove talking marks
            switchValue = switchValue.Replace("\"", "");

            if (value != "") {
                value = switchValue + value;
            }

            return value;
        }

        private static string ProcessTextAfter(string switchValue, string value) {

            // Remove talking marks
            switchValue = switchValue.Replace("\"", "");

            if (value != "") {
                value =  value + switchValue ;
            }

            return value;
        }

        private static string ProcessNumericSwitch(string switchValue, string value)
        {
            bool validNumber = double.TryParse(value, out double dblValue);

            // Remove talking marks
            switchValue = switchValue.Replace("\"", "");

            if (validNumber)
            {
                try
                {
                    value = String.Format("{0:" + switchValue + "}", dblValue);
                }
                catch (FormatException)
                { }
            }

            return value;
        }

        private static string ProcessDateTimeSwitch(string switchValue, string value)
        {
            DateTimeOffset dtmValue = new DateTimeOffset();
            bool validDateTime = DateTimeOffset.TryParse(value, out dtmValue);

            // Remove talking marks
            switchValue = switchValue.Replace("\"", "");

            if (validDateTime)
            {
                try
                {
                    value = String.Format("{0:" + switchValue + "}", dtmValue);
                }
                catch (FormatException)
                { }
            }

            return value;
        }

        private static string UppercaseFirst(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            char[] a = value.ToCharArray();
            a[0] = char.ToUpper(a[0]);

            return new string(a);
        }

        private static bool IsUppercaseFirst(string value)
        {
            bool isUpperCase = false;

            if (!string.IsNullOrEmpty(value))
            {
                char[] a = value.ToCharArray();

                if (a[0] == char.ToUpper(a[0]))
                {
                    isUpperCase = true;
                }
            }

            return isUpperCase;
        }
    }
}
