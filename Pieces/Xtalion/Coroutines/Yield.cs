#region Author

//// Rob Eisenberg, Blue Spire Consulting, Inc 
//// Yevhen Bobrov, http://blog.xtalion.com 

#endregion

using System;
using System.Collections.Generic;
using System.Threading;

namespace Xtalion.Coroutines
{
	public class Yield
	{
		readonly ActionConductor conductor;
		readonly IEnumerator<IAction> iterator;

		Yield(IEnumerable<IAction> routine, ActionConductor conductor)
		{
			iterator = routine.GetEnumerator();
			this.conductor = conductor;
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
				conductor.Complete();
				return;
			}

			IAction next = iterator.Current;
			next.Completed += ActionCompleted;

			conductor.Execute(next);
		}

		public static void Call(IEnumerable<IAction> routine)
		{		
			new Yield(routine, ActionConductor.Default).Iterate();
		}

		public static void Wait(IEnumerable<IAction> routine)
		{
			using (var wait = new BlockingConductor())
			{
				new Yield(routine, wait).Iterate();
				wait.Begin();
			}
		}

		class ActionConductor
		{
			public static readonly ActionConductor Default = new ActionConductor();

			public virtual void Execute(IAction action)
			{
				action.Execute();
			}

			public virtual void Begin()
			{}

			public virtual void Complete()
			{}
		}

		class BlockingConductor : ActionConductor, IDisposable
		{
			ManualResetEvent signal;

			public override void Execute(IAction action)
			{
				DisableDispatch(action as DispatchAction);
				base.Execute(action);
			}

			public override void Begin()
			{
				signal = new ManualResetEvent(false);
				signal.WaitOne();
			}

			public override void Complete()
			{
				signal.Set();
			}

			static void DisableDispatch(DispatchAction action)
			{
				if (action != null)
					action.Dispatch = false;
			}

			public void Dispose()
			{
				signal.Dispose();
			}
		}
	}
}