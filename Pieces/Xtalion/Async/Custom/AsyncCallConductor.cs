using System;

namespace Xtalion.Async.Custom
{
	public abstract class AsyncCallConductor
	{
		public event EventHandler Completed;
		
		bool used;

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
			if (used)
				throw new InvalidOperationException(
					"An instance of Conductor should only be used on per-call basis");

			used = true;
			EventHandler handler = Completed;

			if (handler != null)
				handler(this, EventArgs.Empty);
		}
	}
}
