#region Author

//// Yevhen Bobrov, http://blog.xtalion.com 

#endregion

using System;
using System.Linq.Expressions;
using System.ServiceModel;

namespace Xtalion.Silverlight.Services
{
	public class ServiceCallBuilder<TService> where TService : class
	{
		readonly ServiceChannelFactory<TService> factory;

		public ServiceCallBuilder(string address)
		{
			factory = new ServiceChannelFactory<TService>(new EndpointAddress(address));
		}

		public ServiceCommand<TService> Command(Expression<Action<TService>> expression)
		{
			return new ServiceCommand<TService>(factory, expression);
		}

		public ServiceQuery<TResponse, TService> Query<TResponse>(Expression<Func<TService, TResponse>> expression)
		{
			return new ServiceQuery<TResponse, TService>(factory, expression);
		}
	}
}