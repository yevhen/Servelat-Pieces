#region Author

//// Yevhen Bobrov, http://blog.xtalion.com 

#endregion

using System;
using System.Threading;

namespace Xtalion.Coroutines
{
	public static class Sleep
	{
		public static IAction Timeout(TimeSpan timeout)
		{
			return new SleepAction(timeout);
		}

		private class SleepAction : IAction
		{
			readonly TimeSpan timeout;
			readonly Timer timer;

			public event EventHandler Completed;

			public SleepAction(TimeSpan timeout)
			{
				this.timeout = timeout;
				timer = new Timer(OnTimeout);
			}

			public void Execute()
			{
				timer.Change(timeout, TimeSpan.FromMilliseconds(-1));
			}

			void OnTimeout(object state)
			{
				timer.Dispose();
				SignalCompleted();
			}

			void SignalCompleted()
			{
				EventHandler handler = Completed;

				if (handler != null)
					Dispatcher.Current.Invoke(() => handler(this, EventArgs.Empty));
			}
		}
	}
}
