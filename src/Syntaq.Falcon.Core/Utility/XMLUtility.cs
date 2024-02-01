using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Syntaq.Falcon.Utility
{
	public static class XMLUtility
	{
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

		public static string ReplaceBracketValuesXML(Match Match, XElement Data, string File)
		{
			string fldname = Match.Value.Replace("{", "").Replace("}", "");
			string fldvalue = GetElementValue(Data, fldname);
			return File.Replace(Match.Value, fldvalue);
		}

		//public static string GetElementValueRecersive(XElement File, string fldname)
		//{
		//XElement ElementMatch = new XElement("null");
		//string MatchValue = null;
		//File.Elements().ToList().ForEach(q => { });
		//foreach (XElement ChildElement in File.Elements())
		//{
		//    if (ChildElement.HasElements)
		//    {
		//        MatchValue = GetElementValueRecersive(ChildElement, fldname);
		//        if(MatchValue != null)
		//        {
		//            return MatchValue;
		//        }
		//    }
		//    else
		//    {
		//        if(ChildElement.Name == fldname)
		//        {
		//            MatchValue = ChildElement.Value;
		//            return MatchValue;
		//        }  
		//    }
		//}
		//return MatchValue != null ? MatchValue : "";
		//}

		//GetAttributeValue

		//GetElementValueAsBool

		//SetElementValue
	}
}
