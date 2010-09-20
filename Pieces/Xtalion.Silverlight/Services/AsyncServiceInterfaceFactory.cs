#region Author

//// Yevhen Bobrov, http://blog.xtalion.com 

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Xtalion.Silverlight.Services
{
	public class AsyncServiceInterfaceFactory
	{
		static readonly Lazy<AsyncServiceInterfaceFactory> singleton =
				new Lazy<AsyncServiceInterfaceFactory>(() => new AsyncServiceInterfaceFactory(), true);

		public static AsyncServiceInterfaceFactory Instance
		{
			get { return singleton.Value; }
		}

		readonly ModuleBuilder module;
		readonly Dictionary<Type, Type> generated = new Dictionary<Type, Type>();

		public AsyncServiceInterfaceFactory()
		{
			var assemblyName = new AssemblyName("Xtalion.Silverlight.Services.Auto");

			AppDomain appDomain = AppDomain.CurrentDomain;
			AssemblyBuilder assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

			module = assemblyBuilder.DefineDynamicModule("Xtalion.Silverlight.Services.Auto.dll");
		}

		public Type Generate(Type syncInterface)
		{
			lock (generated)
			{
				if (!generated.ContainsKey(syncInterface))
					generated[syncInterface] = GenerateAsyncInterface(syncInterface);

				return generated[syncInterface];
			}
		}

		Type GenerateAsyncInterface(Type syncInterface)
		{
			var generator = new AsyncServiceInterfaceGenerator(module, syncInterface);
			return generator.Generate();
		}
	}
}
