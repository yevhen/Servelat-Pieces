using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Windows.Forms;

using Sample.Custom.Async;

using Xtalion.Async;
using Xtalion.Coroutines;

namespace Sample.Custom
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		void downloadButton_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(urlTextBox.Text))
				return;

			Yield.Call(BackgroundDownload(new Uri(urlTextBox.Text), queueListBox.Items.Count));
		}

		IEnumerable<IAction> BackgroundDownload(Uri uri, int index)
		{
			queueListBox.Items.Add("Downloading ... -> " + uri);

			var query = Async.Query(webClient => webClient.DownloadString(uri));
			yield return query;

			if (query.Failed)
			{
				queueListBox.Items[index] = string.Format("Failed: {0} -> {1}", query.Exception.Message, uri);
				yield break;
			}

			string fileName = string.Format(@"C:\Temp\Async_{0}.html", index);
			yield return Async.Command(() => File.WriteAllText(fileName, query.Result));

			queueListBox.Items[index] = "Done -> " + uri;
		}

		#region Async

		static class Async //< ---  this wrapper here is just to make BackgroundDownload method above look more consistent on API calls
		{
			public static CustomAsyncQuery<WebClientConductor, IWebClient, TResult> Query<TResult>(Expression<Func<IWebClient, TResult>> expression)
			{
				return new WebClientAsyncCallBuilder().Query(expression);
			}

			public static IAction Command(Expression<Action> expression)
			{
				return new AsyncCommand(expression);
			}
		}

		#endregion
	}
}
