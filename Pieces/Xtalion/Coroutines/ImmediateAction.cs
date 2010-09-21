using System;

namespace Xtalion.Coroutines
{
	public abstract class ImmediateAction : IAction
	{
		public event EventHandler Completed;

		public void Execute()
		{
			OnExecute();
			SignalCompleted();
		}

		protected void SignalCompleted()
		{
			EventHandler handler = Completed;

			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		protected abstract void OnExecute();
	}
}
