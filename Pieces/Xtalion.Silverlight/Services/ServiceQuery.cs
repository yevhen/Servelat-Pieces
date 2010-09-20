#region Author

//// Yevhen Bobrov, http://blog.xtalion.com 

#endregion

using System;
using System.Linq.Expressions;

namespace Xtalion.Silverlight.Services
{
	public class ServiceQuery<TResult, TService> : ServiceCall<TService>
	{
		public ServiceQuery(ServiceChannelFactory<TService> factory, Expression<Func<TService, TResult>> call)
			: base(factory, (MethodCallExpression) call.Body)
		{}

		public TResult Result
		{
			get; private set;
		}

		protected override void HandleResult(object result)
		{
			Result = (TResult) result;
		}
	}
}
