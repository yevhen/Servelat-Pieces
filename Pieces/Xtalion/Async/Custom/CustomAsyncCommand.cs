using System;
using System.Linq.Expressions;

namespace Xtalion.Async.Custom
{
	public class CustomAsyncCommand<TConductor, TSync> : AsyncCall where TConductor : AsyncCallConductor, TSync
	{
		readonly TConductor conductor;
		readonly Expression<Action<TSync>> expression;

		public CustomAsyncCommand(TConductor conductor, Expression<Action<TSync>> expression)
		{
			this.conductor = conductor;
			this.expression = expression;
		}

		public override void Execute()
		{
			conductor.Completed += OnCallCompleted;
			expression.Compile().Invoke(conductor);
		}

		void OnCallCompleted(object sender, EventArgs e)
		{
			Exception = conductor.Exception;
			SignalCompleted();
		}
	}
}
