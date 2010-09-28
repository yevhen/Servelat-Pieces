using System;
using System.Linq.Expressions;
using System.Net;

using Xtalion.Async.Custom;

namespace Sample.Desktop
{
	public interface IWebClient
	{
		byte[] DownloadData(Uri address);
		string DownloadString(Uri address);
		void DownloadFile(Uri address, string fileName);
	}

	public class WebClientCallBuilder
	{
		readonly Func<WebClient> factory;

		public WebClientCallBuilder(Func<WebClient> factory)
		{
			this.factory = factory;
		}

		public CustomAsyncCommand<WebClientConductor, IWebClient> Command(Expression<Action<IWebClient>> expression)
		{
			return new CustomAsyncCommand<WebClientConductor, IWebClient>(new WebClientConductor(factory()), expression);
		}

		public CustomAsyncQuery<WebClientConductor, IWebClient, TResult> Query<TResult>(Expression<Func<IWebClient, TResult>> expression)
		{
			return new CustomAsyncQuery<WebClientConductor, IWebClient, TResult>(new WebClientConductor(factory()), expression);
		}
	}

	public class WebClientConductor : AsyncCallConductor, IWebClient
	{
		readonly WebClient client;

		public WebClientConductor(WebClient client)
		{
			this.client = client;
		}

		public byte[] DownloadData(Uri address)
		{
			client.DownloadDataCompleted += (sender, e) =>
			{
				Exception = e.Error;
				Result = e.Result;
				Complete();
			};

			client.DownloadDataAsync(address);

			return null;
		}

		public void DownloadFile(Uri address, string fileName)
		{
			client.DownloadFileCompleted += (sender, e) =>
			{
				Exception = e.Error;
				Complete();
			};

			client.DownloadFileAsync(address, fileName);
		}

		public string DownloadString(Uri address)
		{
			client.DownloadStringCompleted += (sender, e) =>
			{
				Exception = e.Error;
				Result = e.Result;
				Complete();
			};

			client.DownloadStringAsync(address);

			return null;
		}
	}
}
