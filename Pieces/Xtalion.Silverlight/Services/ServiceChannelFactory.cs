#region Author

//// Yevhen Bobrov, http://blog.xtalion.com 

#endregion

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Xtalion.Silverlight.Services
{
	public class ServiceChannelFactory<TService>
	{
		dynamic channelFactory;

		public ServiceChannelFactory(Binding binding, EndpointAddress address)
		{
			BuildChannelFactory(binding, address);
		}

		public ServiceChannelFactory(EndpointAddress address)
		{
			BuildChannelFactory(new BasicHttpBinding(), address);
		}

		void BuildChannelFactory(Binding binding, EndpointAddress address)
		{
			channelFactory = Activator.CreateInstance(GetChannelFactoryType(), binding, address);			
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
