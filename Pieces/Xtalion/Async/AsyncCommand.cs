using System;
using System.Linq.Expressions;

using Xtalion.Async.Execution;

namespace Xtalion.Async
{
	public class AsyncCommand : AsyncCall
	{
		readonly ExecutionMethod method;

		public AsyncCommand(Expression<Action> expression, bool apm = false)
		{
			method = apm 
				? ExecutionMethod.UseApmMethodPair(this, expression) 
				: ExecutionMethod.QueueUserWorkItem(this, expression);
		}

		public override void Execute()
		{
			method.Execute();
		}
	}
}
