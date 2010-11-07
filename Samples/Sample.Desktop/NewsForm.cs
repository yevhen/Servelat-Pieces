using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;

using Xtalion.Async;
using Xtalion.Coroutines;

namespace Sample.Desktop
{
	public partial class NewsForm : Form
	{
		readonly NewsFeed news = new NewsFeed();

		public NewsForm()
		{
			InitializeComponent();
		}

		private void NewsForm_Load(object sender, EventArgs e)
		{
			Yield.Call(UpdateNews());
		}

		IEnumerable<IAction> UpdateNews()
		{
			while (true)
			{
				var query = new AsyncQuery<string>(() => news.Download(5));
				yield return query;

				if (!query.Failed)
					ticker.ScrollText = query.Result;

				yield return Sleep.Timeout(TimeSpan.FromSeconds(40));
			}
		}
	}
}
