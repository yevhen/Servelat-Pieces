using System;

using Xtalion.Coroutines;

namespace Xtalion.Silverlight
{
	public class MainThreadDispatcher : IDispatcher
	{
		public void Invoke(Action action)
		{
			Call.OnUIThread(action);
		}
	}
}
