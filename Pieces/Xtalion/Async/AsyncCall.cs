using System;

using Xtalion.Coroutines;

namespace Xtalion.Async
{
	public abstract class AsyncCall : DispatchAction
	{
		public bool Failed
		{
			get { return Exception != null; }
		}

		public Exception Exception
		{
			get; protected set;
		}
	}
}
