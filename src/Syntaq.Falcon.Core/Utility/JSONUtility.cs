using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Syntaq.Falcon.Utility
{
	public static class JSONUtility
	{
		public static List<JToken> FindMatchingTokens(this JObject containerToken, string key, string value)
		{
			List<JToken> matches = new List<JToken>();
			FindTokens(containerToken, key, value, matches);
			return matches;
		}

		public static bool HasMatchingToken(this JObject containerToken, string key, string value)
		{
			bool HasMatch = false;
			FindMatchingToken(containerToken, key, value, ref HasMatch);
			return HasMatch;
		}

		public static bool HasMatchingToken(this JObject containerToken, string key)
		{
			bool HasMatch = false;
			FindMatchingToken(containerToken, key, "", ref HasMatch);
			return HasMatch;
		}

		private static void FindTokens(JToken containerToken, string key, string value, List<JToken> matches)
		{
			if (containerToken.Type == JTokenType.Object)
			{
				foreach (JProperty child in containerToken.Children<JProperty>())
				{
					if (child.Name == key)
					{
						if (Convert.ToString(child.Value) == value)
						{
							//var k = child.Parent;
							//var l = k;
							//matches.Add(child.Value);
							matches.Add(child.Parent);
						}
					}
					FindTokens(child.Value, key, value, matches);
				}
			}
			else if (containerToken.Type == JTokenType.Array)
			{
				foreach (JToken child in containerToken.Children())
				{
					FindTokens(child, key, value, matches);
				}
			}
		}

		private static void FindMatchingToken(JToken containerToken, string key, string value, ref bool HasMatch)
		{
			if (containerToken.Type == JTokenType.Object)
			{
				foreach (JProperty child in containerToken.Children<JProperty>())
				{
					if (child.Name == key)
					{
						if (string.IsNullOrEmpty(value))
						{
							HasMatch = true;
						}
						if (Convert.ToString(child.Value) == value)
						{
							HasMatch = true;
						}
					}
					FindMatchingToken(child.Value, key, value, ref HasMatch);
				}
			}
			else if (containerToken.Type == JTokenType.Array)
			{
				foreach (JToken child in containerToken.Children())
				{
					FindMatchingToken(child, key, value, ref HasMatch);
				}
			}
		}
	}
}