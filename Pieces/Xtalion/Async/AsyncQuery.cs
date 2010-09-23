using System;
using System.Linq.Expressions;

using Xtalion.Async.Execution;

namespace Xtalion.Async
{
	public class AsyncQuery<TResult> : AsyncCall<TResult>
	{
		readonly ExecutionMethod method;
		
		public AsyncQuery(Expression<Func<TResult>> expression, bool apm = false)
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
