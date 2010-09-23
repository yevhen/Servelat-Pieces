#region Author

//// Yevhen Bobrov, http://blog.xtalion.com 

#endregion

using System;
using System.Linq.Expressions;

using Xtalion.Async;
using Xtalion.Async.Execution;

namespace Xtalion.Silverlight.Services
{
	public class ServiceQuery<TService, TResult> : AsyncCall<TResult>
	{
		readonly ApmInvocation invocation;

		public ServiceQuery(ServiceChannelFactory<TService> factory, Expression<Func<TService, TResult>> expression)
		{
			invocation = new ApmInvocation(factory.CreateChannel(), (MethodCallExpression)expression.Body)
			{
				End = (sender, args) =>
				{
					Result = (TResult) invocation.Result;
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
