using System;
using System.Linq.Expressions;

namespace Xtalion.Async.Execution
{
	abstract class ExecutionMethod
	{
		public abstract void Execute();

		public static ExecutionMethod QueueUserWorkItem(AsyncCall owner, Expression<Action> expression)
		{
			return new WorkItemExecutionMethod(owner, expression.Compile());
		}

		public static ExecutionMethod UseApmMethodPair(AsyncCall owner, Expression<Action> expression)
		{
			return new ApmExecutionMethod(owner, (MethodCallExpression)expression.Body);
		}

		public static ExecutionMethod QueueUserWorkItem<TResult>(AsyncCall<TResult> owner, Expression<Func<TResult>> expression)
		{
			return new WorkItemExecutionMethod<TResult>(owner, expression.Compile());
		}

		public static ExecutionMethod UseApmMethodPair<TResult>(AsyncCall<TResult> owner, Expression<Func<TResult>> expression)
		{
			return new ApmExecutionMethod<TResult>(owner, (MethodCallExpression)expression.Body);
		}
	}
}
