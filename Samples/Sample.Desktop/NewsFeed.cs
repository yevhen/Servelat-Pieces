using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Sample.Desktop
{
	public class NewsFeed
	{
		const string url = "http://mobile.reuters.com/reuters/rss/TOP.xml";

		public string Download(int top)
		{
			string timestamp = DateTime.Now.ToString("HH:mm:ss");

			XElement rss = XElement.Load(url);
			IEnumerable<XElement> titles = rss.Descendants("item").Descendants("title");

			return timestamp + ":   " +  string.Join("      ", 
					titles.Take(top).Select(x => (string)x).ToArray());
		}
	}
}
