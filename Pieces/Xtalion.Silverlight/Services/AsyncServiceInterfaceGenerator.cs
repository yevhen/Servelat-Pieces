#region Author

//// Yevhen Bobrov, http://blog.xtalion.com 

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.ServiceModel;

namespace Xtalion.Silverlight.Services
{
	public class AsyncServiceInterfaceGenerator
	{
		readonly ModuleBuilder module;
		readonly Type syncInterface;

		TypeBuilder asyncInterface;

		public AsyncServiceInterfaceGenerator(ModuleBuilder module, Type syncInterface)
		{
			this.module = module;
			this.syncInterface = syncInterface;
		}

		public Type Generate()
		{
			DefineAsyncInterfaceType();
			DefineAsyncMethodPairs();

			return asyncInterface.CreateType();
		}

		void DefineAsyncInterfaceType()
		{
			if (!HasAttribute(syncInterface, typeof(ServiceContractAttribute)))
				throw new InvalidOperationException("Can't build asynchronous proxy for type without ServiceContract attribute");

			asyncInterface = module.DefineType(syncInterface.Namespace + "." + syncInterface.Name + "Async",
			                                   TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);

			var serviceContractAttribute = GetCustomAttribute<ServiceContractAttribute>(syncInterface);

			asyncInterface.SetCustomAttribute(
				CreateAttribute<ServiceContractAttribute>(new[] { "Name", "Namespace"}, new[] { syncInterface.Name, serviceContractAttribute.Namespace}));
		}

		void DefineAsyncMethodPairs()
		{
			foreach (MethodInfo method in GetOperationContractMethods())
			{
				DefineAsyncMethodPair(method);
			}
		}

		void DefineAsyncMethodPair(MethodInfo method)
		{
			DefineBeginMethod(method);
			DefineEndMethod(method);
		}

		void DefineBeginMethod(MethodInfo method)
		{
			List<Type> parameterTypes = CollectParameterTypes(method);

			MethodBuilder methodBuilder = asyncInterface.DefineMethod("Begin" + method.Name, MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual,
														  typeof(IAsyncResult), parameterTypes.ToArray());
            DefineParameterNames(method, methodBuilder);

			DefineOperationContractAttribute(methodBuilder);
			DefineFaultContractAttribute(method, methodBuilder);
		}

	    static void DefineParameterNames(MethodInfo syncMethod, MethodBuilder asyncMethod)
	    {
	        int position = 1;
	        foreach (ParameterInfo parameter in syncMethod.GetParameters())
	        {
	            asyncMethod.DefineParameter(position++, ParameterAttributes.In, parameter.Name);
	        }
	    }

	    static List<Type> CollectParameterTypes(MethodInfo method)
		{
			return new List<Type>(method.GetParameters().Select(parameter => parameter.ParameterType))
			{
				typeof(AsyncCallback), typeof(object)
			};
		}

		static void DefineOperationContractAttribute(MethodBuilder methodBuilder)
		{
			methodBuilder.SetCustomAttribute(CreateAttribute<OperationContractAttribute>("AsyncPattern", true));
		}

		static void DefineFaultContractAttribute(MethodInfo method, MethodBuilder methodBuilder)
		{
			if (!HasAttribute(method, typeof(FaultContractAttribute)))
				return;

			var faultContract = (FaultContractAttribute)method.GetCustomAttributes(typeof(FaultContractAttribute), true)[0];

			var attributeBulder = new CustomAttributeBuilder(
				typeof(FaultContractAttribute).GetConstructor(new[] { typeof(Type) }), new object[] { faultContract.DetailType });

			methodBuilder.SetCustomAttribute(attributeBulder);
		}

		void DefineEndMethod(MethodInfo method)
		{
			asyncInterface.DefineMethod("End" + method.Name, MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual,
										method.ReturnType, new[] { typeof(IAsyncResult) });
		}

		IEnumerable<MethodInfo> GetOperationContractMethods()
		{
			return syncInterface.GetMethods().Where(method => HasAttribute(method, typeof(OperationContractAttribute)));
		}

		static bool HasAttribute(ICustomAttributeProvider provider, Type attribute)
		{
			return provider.GetCustomAttributes(attribute, true).Length > 0;
		}

		static TAttribute GetCustomAttribute<TAttribute>(ICustomAttributeProvider provider)
		{
			return (TAttribute) provider.GetCustomAttributes(typeof(TAttribute), true)[0];
		}

		static CustomAttributeBuilder CreateAttribute<TAttribute>(string property, object value) where TAttribute : Attribute
		{
			return CreateAttribute<TAttribute>(new[] {property}, new[] {value});
		}

		static CustomAttributeBuilder CreateAttribute<TAttribute>(string[] properties, object[] values) where TAttribute : Attribute
		{
			if (properties.Length != values.Length)
				throw new ArgumentException("The length of 'properties' and 'values' array parameters should match");

			Type attributeType = typeof(TAttribute);

			return new CustomAttributeBuilder(attributeType.GetConstructor(new Type[0]), new object[0],
											  properties.Select(attributeType.GetProperty).ToArray(), values);
		}
	}
}
