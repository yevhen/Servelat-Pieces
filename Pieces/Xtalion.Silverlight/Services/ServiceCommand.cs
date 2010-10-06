#region Author

//// Yevhen Bobrov, http://blog.xtalion.com 

#endregion

using System;
using System.Linq.Expressions;

using Xtalion.Async;

namespace Xtalion.Silverlight.Services
{
	public class ServiceCommand<TService> : AsyncCall
	{
		readonly ApmInvocation invocation;

		public ServiceCommand(ServiceChannelFactory<TService> factory, Expression<Action<TService>> expression)
		{
			invocation = new ApmInvocation(factory.CreateChannel(), (MethodCallExpression) expression.Body)
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
