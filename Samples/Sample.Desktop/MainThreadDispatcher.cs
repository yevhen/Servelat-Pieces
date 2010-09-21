using System;
using System.Threading;

using Xtalion.Coroutines;

namespace Sample.Desktop
{
	public class MainThreadDispatcher : IDispatcher
	{
		SynchronizationContext context;

		public MainThreadDispatcher()
		{
			context = SynchronizationContext.Current;
		}

		public void Invoke(Action action)
		{
			context.Send(x => action(), null);
		}
	}
}
