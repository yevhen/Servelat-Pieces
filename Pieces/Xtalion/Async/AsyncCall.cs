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
			get; protected internal set;
		}
	}

	public abstract class AsyncCall<TResult> : AsyncCall
	{
		TResult result;

		public TResult Result
		{
			get
			{
				if (Failed)
					throw new InvalidOperationException("Action failed. Check failure before getting result");

				return result;
			}
			protected internal set { result = value; }
		}
	}
}
