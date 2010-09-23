using System;
using System.Threading;

namespace Xtalion.Async.Execution
{
	internal abstract class WorkItemExecutionMethodBase : ExecutionMethod
	{
		readonly AsyncCall owner;

		protected WorkItemExecutionMethodBase(AsyncCall owner)
		{
			this.owner = owner;
		}

		public override void Execute()
		{
			ThreadPool.QueueUserWorkItem(ExecuteCall, null);
		}

		void ExecuteCall(object state)
		{
			try
			{
				Call();
			}
			catch (Exception exc)
			{
				owner.Exception = exc;
			}

			owner.SignalCompleted();
		}

		protected abstract void Call();
	}

	internal class WorkItemExecutionMethod : WorkItemExecutionMethodBase
	{
		readonly Action call;

		public WorkItemExecutionMethod(AsyncCall owner, Action call) : base(owner)
		{
			this.call = call;
		}

		protected override void Call()
		{
			call();
		}
	}

	internal class WorkItemExecutionMethod<TResult> : WorkItemExecutionMethodBase
	{
		readonly AsyncCall<TResult> owner;
		readonly Func<TResult> call;

		public WorkItemExecutionMethod(AsyncCall<TResult> owner, Func<TResult> call) : base(owner)
		{
			this.owner = owner;
			this.call = call;
		}

		protected override void Call()
		{
			owner.Result = call();
		}
	}
}
