#region Author

//// Yevhen Bobrov, http://blog.xtalion.com 

#endregion

using System;
using System.Linq.Expressions;

using Xtalion.Async;

namespace Xtalion.Silverlight.Services
{
	public sealed class ServiceCommand<TService> : AsyncCall
	{
		readonly Invocation invocation;

		public ServiceCommand(ServiceChannelFactory<TService> factory, TService instance, Expression<Action<TService>> expression)
		{
			invocation = ServiceCall<TService>.MakeInvocation(factory, instance, (MethodCallExpression) expression.Body);

			invocation.Completed = (sender, args) =>
			{
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
