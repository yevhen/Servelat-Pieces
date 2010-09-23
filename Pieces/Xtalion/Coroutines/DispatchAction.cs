using System;

namespace Xtalion.Coroutines
{
	public abstract class DispatchAction : IAction
	{
		public event EventHandler Completed;

		protected internal void SignalCompleted()
		{
			EventHandler handler = Completed;

			if (handler != null)
				Dispatcher.Current.Invoke(() => handler(this, EventArgs.Empty));
		}

		public abstract void Execute();	
	}
}
