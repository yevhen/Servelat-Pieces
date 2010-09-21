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

		private class SleepAction : DispatchAction
		{
			readonly TimeSpan timeout;
			readonly Timer timer;

			public SleepAction(TimeSpan timeout)
			{
				this.timeout = timeout;
				timer = new Timer(OnTimeout);
			}

			public override void Execute()
			{
				timer.Change(timeout, TimeSpan.FromMilliseconds(-1));
			}

			void OnTimeout(object state)
			{
				timer.Dispose();
				SignalCompleted();
			}
		}
	}
}
