#region Author

//// Rob Eisenberg, Blue Spire Consulting, Inc 

#endregion

using System;
using System.Collections.Generic;
using System.Threading;

namespace Xtalion.Coroutines
{
	public class Yield
	{
		readonly ManualResetEvent wait;
		readonly IEnumerator<IAction> iterator;

		Yield(IEnumerator<IAction> iterator, ManualResetEvent wait)
		{
			this.iterator = iterator;
			this.wait = wait;
		}

		void Iterate()
		{
			ActionCompleted(null, EventArgs.Empty);
		}

		void ActionCompleted(object sender, EventArgs args)
		{
			var previous = sender as IAction;

			if (previous != null)
				previous.Completed -= ActionCompleted;

			if (!iterator.MoveNext())
			{
				Complete();
				return;
			}

			IAction next = iterator.Current;
			next.Completed += ActionCompleted;

			Execute(next);
		}

		void Complete()
		{
			if (wait != null)
				wait.Set();
		}

		void Execute(IAction action)
		{
			if (wait != null)
				DisableDispatch(action as DispatchAction);

			action.Execute();
		}

		static void DisableDispatch(DispatchAction action)
		{
			if (action != null)
				action.Dispatch = false;
		}

		public static void Routine(IEnumerable<IAction> routine, bool wait = false)
		{
			ManualResetEvent signal = wait ? new ManualResetEvent(false) : null;
			
			new Yield(routine.GetEnumerator(), signal).Iterate();

			if (signal != null)
			{
				signal.WaitOne();
				signal.Dispose();
			}
		}
	}
}