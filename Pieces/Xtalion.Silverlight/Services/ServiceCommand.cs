#region Author

//// Yevhen Bobrov, http://blog.xtalion.com 

#endregion

using System;
using System.Linq.Expressions;

namespace Xtalion.Silverlight.Services
{
	public class ServiceCommand<TService> : ServiceCall<TService>
	{
		public ServiceCommand(ServiceChannelFactory<TService> factory, Expression<Action<TService>> call)
			: base(factory, (MethodCallExpression) call.Body)
		{}

		protected override void HandleResult(object result)
		{
			// do nothing - commands are void
		}
	}
}
