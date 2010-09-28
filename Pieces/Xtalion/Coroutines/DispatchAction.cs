using System;
using System.Threading;

namespace Xtalion.Coroutines
{
	public abstract class DispatchAction : IAction
	{
		public event EventHandler Completed;

		readonly SynchronizationContext dispatcher = SynchronizationContext.Current;

		protected DispatchAction()
		{
			Dispatch = true;
		}

		internal bool Dispatch
		{
			get; set;
		}

		protected internal void SignalCompleted()
		{
			EventHandler handler = Completed;

			if (handler == null)
				return;

			if (dispatcher == null || !Dispatch)
			{
				handler(this, EventArgs.Empty);
				return;
			}

			dispatcher.Post(x => handler(this, EventArgs.Empty), null);
		}

		public abstract void Execute();
	}
}
