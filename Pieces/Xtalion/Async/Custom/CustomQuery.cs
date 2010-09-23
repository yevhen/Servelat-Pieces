using System;
using System.Linq.Expressions;

namespace Xtalion.Async.Custom
{
	public class CustomQuery<TConductor, TSync, TResult> : AsyncCall<TResult> where TConductor : CallConductor, TSync
	{
		readonly TConductor conductor;
		readonly Expression<Func<TSync, TResult>> expression;

		public CustomQuery(TConductor conductor, Expression<Func<TSync, TResult>> expression)
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
			Result = (TResult) conductor.Result;

			SignalCompleted();
		}
	}
}
