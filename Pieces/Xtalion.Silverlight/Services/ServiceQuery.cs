#region Author

//// Yevhen Bobrov, http://blog.xtalion.com 

#endregion

using System;
using System.Linq.Expressions;

using Xtalion.Async;

namespace Xtalion.Silverlight.Services
{
	public sealed class ServiceQuery<TService, TResult> : AsyncCall<TResult>
	{
		readonly Invocation invocation;

		public ServiceQuery(ServiceChannelFactory<TService> factory, TService instance, Expression<Func<TService, TResult>> expression)
		{
			invocation = ServiceCall<TService>.MakeInvocation(factory, instance, (MethodCallExpression)expression.Body);
			
			invocation.Completed = (sender, args) =>
			{
				Result = (TResult) invocation.Result;
				Exception = invocation.Exception;

				SignalCompleted();
			};
		}

		public override void Execute()
		{
			invocation.Invoke();
		}
	}
}
