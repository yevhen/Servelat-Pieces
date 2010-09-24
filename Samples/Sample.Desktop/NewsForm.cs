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
		NewsFeed news = new NewsFeed();

		public NewsForm()
		{
			InitializeComponent();
		}

		private void NewsForm_Load(object sender, EventArgs e)
		{
			Run.Sequence(UpdateNews());
			Run.Sequence(BackgroundDownload());
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

		static IEnumerable<IAction> BackgroundDownload()
		{
			var build = new WebClientCallBuilder(() => new WebClient());
			var uri = new Uri("http://www.codeproject.com/");

			var query = build.Query(client => client.DownloadString(uri));
			var cmd = build.Command(client => client.DownloadFile(uri, @"C:\Temp\2.html"));

			yield return Parallel.Actions(query, cmd);

			if (!query.Failed)
				File.WriteAllText(@"C:\Temp\1.html", query.Result);
		}
	}
}
