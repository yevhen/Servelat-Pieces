using System.Linq.Expressions;

namespace Xtalion.Async.Execution
{
	internal class ApmExecutionMethod : ExecutionMethod
	{
		readonly ApmInvocation invocation;

		public ApmExecutionMethod(AsyncCall owner, MethodCallExpression expression)
		{
			invocation = new ApmInvocation(expression)
			{
				End = (sender, args) =>
				{
					HandleResult(invocation.Result);
					
					owner.Exception = invocation.Exception;
					owner.SignalCompleted();
				}
			};
		}

		public override void Execute()
		{
			invocation.Invoke();
		}

		protected virtual void HandleResult(object result) {}
	}

	internal class ApmExecutionMethod<TResult> : ApmExecutionMethod
	{
		readonly AsyncCall<TResult> owner;

		public ApmExecutionMethod(AsyncCall<TResult> owner, MethodCallExpression expression)
			: base(owner, expression)
		{
			this.owner = owner;
		}

		protected override void HandleResult(object result)
		{
			owner.Result = (TResult) result;
		}
	}
}
