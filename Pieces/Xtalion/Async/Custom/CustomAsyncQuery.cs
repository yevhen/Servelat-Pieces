using System;
using System.Linq.Expressions;
using System.Threading;

namespace Xtalion.Async.Custom
{
	public class CustomAsyncQuery<TConductor, TSync, TResult> : AsyncCall<TResult> where TConductor : AsyncCallConductor, TSync
	{
		readonly TConductor conductor;
		readonly Func<TSync, TResult> call;

		public CustomAsyncQuery(TConductor conductor, Expression<Func<TSync, TResult>> expression)
		{
			this.conductor = conductor;
			call = expression.Compile();
		}

		public override void Execute()
		{
			conductor.Completed += OnCallCompleted;
			ThreadPool.QueueUserWorkItem(x => call.Invoke(conductor), null);
		}

		void OnCallCompleted(object sender, EventArgs e)
		{
			Exception = conductor.Exception;
			Result = (TResult) conductor.Result;

			SignalCompleted();
		}
	}
}
