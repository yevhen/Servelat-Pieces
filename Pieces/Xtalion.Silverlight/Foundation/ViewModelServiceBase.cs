using System;
using System.Linq.Expressions;

using Xtalion.Silverlight.Services;

namespace Xtalion.Silverlight
{
	public class ViewModelServiceBase<TService> : ViewModelBase where TService : class
	{
		readonly ServiceCallBuilder<TService> build;

		protected ViewModelServiceBase(string address)
		{
			build = new ServiceCallBuilder<TService>(address);
		}

		protected ServiceCommand<TService> Command(Expression<Action<TService>> call)
		{
			return build.Command(call);
		}

		protected ServiceQuery<TResponse, TService> Query<TResponse>(Expression<Func<TService, TResponse>> call)
		{
			return build.Query(call);
		}
	}
}
