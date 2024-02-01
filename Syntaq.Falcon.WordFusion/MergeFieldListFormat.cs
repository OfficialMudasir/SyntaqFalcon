using System;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;

namespace WordFusion.Assembly.MailMerge {
    
    internal class MergeFieldListFormat {

        public static string FormatListValue(String ListFormat, int rowindex, int rowcount) {

            // a; b; ; and c.
            // a; b; ; and c.
            switch (ListFormat.Trim()) {
                case "a":
                    return " ";
                case "a and b":
                    return FormatType1Get(" and ", rowindex, rowcount);
                case "a, b and c":
                    return FormatType2Get(" and ", ",", rowindex, rowcount);
                case "a, b and c.":
                    return FormatType5Get(" and ", ", ", ".", rowindex, rowcount);
                case "a, b and c,":
                    return FormatType5Get(" and ", ", ", ",", rowindex, rowcount);
                case "a; b; or; c":
                    return FormatType4Get(" or; ", ";", "", rowindex, rowcount);
                case "a; b; ; or c.":
                    return FormatType4Get(" or ", ";", ".", rowindex, rowcount);
                case "a; b; and c.":
                    return FormatType4Get(" and ", ";", ".", rowindex, rowcount);
                case "a; b; c.":
                    return FormatType4Get("", ";", ".", rowindex, rowcount);
                case "a; b; or c.":
                    return FormatType4Get(" or ", ";", ".", rowindex, rowcount);
                case "a; b; and c,":
                    return FormatType4Get(" and ", ";", ",", rowindex, rowcount);
                case "a, b, and c.":
                    return FormatType4Get(" and ", ";", ".", rowindex, rowcount);
                case "a, b, and c,":
                    return FormatType4Get(" and ", ",", ",", rowindex, rowcount);
                case "a; b; and c;":
                    return FormatType4Get(" and ", ";", ";", rowindex, rowcount);
                case "a; b; ; and c;":
                    return FormatType4Get(" and ", ";", ";", rowindex, rowcount);
                case "a; b; ; and c.":
                    return FormatType4Get(" and ", ";", ".", rowindex, rowcount);
                case "a; b; ; and c,":
                    return FormatType4Get(" and ", ";", ",", rowindex, rowcount);
                case "a; b; ;and c;":
                    return FormatType4Get(" and ", ";", ";", rowindex, rowcount);
                case "a; b; ;and c.":
                    return FormatType4Get(" and ", ";", ".", rowindex, rowcount);
                case "a; b; ;and c,":
                    return FormatType4Get(" and ", ";", ",", rowindex, rowcount);
                case "a; b and c":
                    return FormatType2Get(" and ", ";", rowindex, rowcount);
                case "a; and b; and c.":
                    return FormatType3Get(" and ", "; and ", ".", rowindex, rowcount);
                case "a or b":
                    return FormatType1Get(" or ", rowindex, rowcount);
                case "a, b or c":
                    return FormatType2Get(" or ", ",", rowindex, rowcount);
                case "a; b or c":
                    return FormatType2Get(" or ", ";", rowindex, rowcount);
            }

            return "";
        }

        private static String FormatType1Get(String type, int rowindex, int rowcount) {

            if (rowindex == 0 && rowcount == 1) return " ";
            if (rowindex == 0 && rowcount == 2) return type;
            if (rowindex == 0 && rowcount > 2) return type;
            if (rowindex > 0 && rowindex <= rowcount - 2) return type;
            if (rowindex > 0 && rowindex == rowcount - 1) return "";

            return "";

        }

        private static String FormatType2Get(String type, String delim, int rowindex, int rowcount) {

            if (rowindex == 0 && rowcount == 1) return " ";
            if (rowindex == 0 && rowcount == 2) return type;
            if (rowindex == 0 && rowcount > 2) return delim + " ";

            if (rowindex > 0 && rowindex == rowcount - 2) return type;
            if (rowindex > 0 && rowindex <= rowcount - 2) return delim + " ";
            if (rowindex > 0 && rowindex == rowcount - 1) return "";

            return "";

        }

        private static String FormatType3Get(String type, String delim, String term, int rowindex, int rowcount) {

            String retval = "";

            if (rowindex == 0 && rowcount == 1) retval = term;
            if (rowindex == 0 && rowcount == 2) retval = type;
            if (rowindex == 0 && rowcount > 2) retval = delim;

            if (rowindex > 0 && rowindex <= rowcount - 2) retval = delim;
            if (rowindex > 0 && rowindex == rowcount - 1) retval = term;

            return retval;

        }


        private static String FormatType4Get(String type, String delim, String term, int rowindex, int rowcount) {

            String retval = "";

            if (rowindex == 0 && rowcount == 1) retval = term;
            if (rowindex == 0 && rowcount == 2) retval = delim + type;
            if (rowindex == 0 && rowcount > 2 || rowindex > 0 && rowindex <= rowcount - 2) retval = delim;

            if (rowindex > 0 && rowindex == rowcount - 2) retval = delim + type;
            if (rowindex > 0 && rowindex == rowcount - 1) retval = term;

            return retval;

        }

        private static String FormatType5Get(String type, String delim, String term, int rowindex, int rowcount) {

            String retval = "";

            if (rowindex == 0 && rowcount == 1) retval = term;
            if (rowindex == 0 && rowcount == 2) retval = type;
            if (rowindex == 0 && rowcount > 2) retval = delim;

            // if the 
            
            if (rowindex > 0 && rowindex <= rowcount - 2) retval = delim;
            if (rowindex > 0 && rowindex == rowcount - 2) retval = type;
            if (rowindex > 0 && rowindex == rowcount - 1) retval = term;

            return retval;

        }
    }
}
