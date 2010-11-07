using System;
using System.Linq.Expressions;
using System.Threading;

using Xtalion.Async;

namespace Sample.Custom.Async
{
	public class CustomAsyncQuery<TConductor, TSync, TResult> : AsyncCall<TResult> where TConductor : CustomAsyncCallConductor, TSync
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
