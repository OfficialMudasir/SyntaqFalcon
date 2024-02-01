using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Syntaq.Falcon.Utility
{
	public static class StringUtility
	{
		public static string Truncate(this string value, int maxLength)
		{
			if (string.IsNullOrEmpty(value) || value.Length < maxLength)
			{
				return value;
			}
			else
			{
				value = value.Substring(0, maxLength - 4);
				return value += "...";
			}
		}

		public static string ReplaceBracketValuesJSON(Match Match, JObject jdata, string input)
		{
			string fldname = Match.Value.Replace("{", "").Replace("}", "");
			string fldvalue = jdata[fldname] == null ? string.Empty : jdata[fldname].ToString();
			return input.Replace(Match.Value, fldvalue);
		}
	}
}