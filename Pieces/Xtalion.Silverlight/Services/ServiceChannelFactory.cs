#region Author

//// Yevhen Bobrov, http://blog.xtalion.com 

#endregion

using System;
using System.ServiceModel;

namespace Xtalion.Silverlight.Services
{
	public class ServiceChannelFactory<TService>
	{
		dynamic channelFactory;

		public ServiceChannelFactory(EndpointAddress address)
		{
			BuildChannelFactory(address);
		}

		void BuildChannelFactory(EndpointAddress address)
		{
			channelFactory = Activator.CreateInstance(GetChannelFactoryType(), new BasicHttpBinding(), address);			
		}

		static Type GetChannelFactoryType()
		{
			Type type = AsyncServiceInterfaceFactory.Instance.Generate(typeof(TService));
			return typeof(ChannelFactory<>).MakeGenericType(type);
		}

		public object CreateChannel()
		{
			return channelFactory.CreateChannel();
		}
	}
}
