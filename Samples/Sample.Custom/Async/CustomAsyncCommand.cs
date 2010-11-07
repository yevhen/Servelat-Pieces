using System;
using System.Linq.Expressions;
using System.Threading;

using Xtalion.Async;

namespace Sample.Custom.Async
{
	public class CustomAsyncCommand<TConductor, TSync> : AsyncCall where TConductor : CustomAsyncCallConductor, TSync
	{
		readonly TConductor conductor;
		readonly Action<TSync> call;

		public CustomAsyncCommand(TConductor conductor, Expression<Action<TSync>> expression)
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
			SignalCompleted();
		}
	}
}
