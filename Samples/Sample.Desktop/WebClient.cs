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
		readonly IWebClient interfacer;
		readonly Func<WebClient> implementer;

		public WebClientCallBuilder(IWebClient interfacer, Func<WebClient> implementer)
		{
			this.interfacer = interfacer;
			this.implementer = implementer;
		}

		public CustomCommand<WebClientConductor, IWebClient> Command(Expression<Action<IWebClient>> expression)
		{
			return new CustomCommand<WebClientConductor, IWebClient>(new WebClientConductor(implementer()), expression);
		}

		public CustomQuery<WebClientConductor, IWebClient, TResult> Query<TResult>(Expression<Func<IWebClient, TResult>> expression)
		{
			return new CustomQuery<WebClientConductor, IWebClient, TResult>(new WebClientConductor(implementer()), expression);
		}
	}

	public class WebClientConductor : CallConductor, IWebClient
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
