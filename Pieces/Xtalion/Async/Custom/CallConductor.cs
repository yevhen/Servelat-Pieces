using System;

namespace Xtalion.Async.Custom
{
	public abstract class CallConductor
	{
		public event EventHandler Completed;
		bool completed;

		public Exception Exception
		{
			get; protected set;
		}

		public object Result
		{
			get; protected set;
		}

		protected void Complete()
		{
			if (completed)
				throw new InvalidOperationException("An instance of Conductor should only be used on per-call basis");

			completed = true;
			EventHandler handler = Completed;

			if (handler != null)
				handler(this, EventArgs.Empty);
		}
	}
}
