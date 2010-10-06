using System;
using System.Linq.Expressions;

namespace Xtalion.Async
{
	public class AsyncPatternCommand : AsyncCall
	{
		readonly ApmInvocation invocation;

		public AsyncPatternCommand(Expression<Action> expression)
		{
			var call = (MethodCallExpression) expression.Body;

			invocation = new ApmInvocation(call.GetTarget(), call)
			{
				End = (sender, args) =>
				{
					Exception = invocation.Exception;
					SignalCompleted();
				}
			};
		}

		public override void Execute()
		{
			invocation.Invoke();
		}
	}
}
