using System;
using System.Linq.Expressions;

using Xtalion.Async;

namespace Xtalion.Silverlight.Services
{
	static class ServiceCall<TService>
	{
		public static Invocation MakeInvocation(ServiceChannelFactory<TService> factory, TService instance, MethodCallExpression methodCall)
		{
			if (ReferenceEquals(instance, null))
				return new ApmInvocation(factory.CreateChannel(), methodCall);

			return new DirectInvocation(instance, methodCall);
		}
	}
}
