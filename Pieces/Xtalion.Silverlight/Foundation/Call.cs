#region Author

 //// Rob Eisenberg, Blue Spire Consulting, Inc 

#endregion

using System;
using System.Windows;
using System.Windows.Threading;

namespace Xtalion.Silverlight
{
	public static class Call
	{
		public static void OnUIThread(Action action)
		{
			Dispatcher dispatcher = Deployment.Current.Dispatcher;

			if (dispatcher.CheckAccess())
			{
				action();
				return;
			}

			dispatcher.BeginInvoke(action);
		}
	}
}
