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
		readonly TService instance;
		readonly ServiceChannelFactory<TService> factory;

		public ServiceCallBuilder(string address)
			: this(null, address)
		{}

		public ServiceCallBuilder(TService instance, string address)
		{
			this.instance = instance;
			factory = new ServiceChannelFactory<TService>(new EndpointAddress(address));
		}

		public ServiceCommand<TService> Command(Expression<Action<TService>> expression)
		{
			return new ServiceCommand<TService>(factory, instance, expression);
		}

		public ServiceQuery<TService, TResponse> Query<TResponse>(Expression<Func<TService, TResponse>> expression)
		{
			return new ServiceQuery<TService, TResponse>(factory, instance, expression);
		}
	}
}