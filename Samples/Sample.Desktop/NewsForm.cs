using System;
using System.Collections.Generic;
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
			InitializeInfrastructure();			
		}

		static void InitializeInfrastructure()
		{
			Dispatcher.Current = new MainThreadDispatcher();
		}

		private void NewsForm_Load(object sender, EventArgs e)
		{			
			Run.Sequence(UpdateNews());
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
