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
			string asyncInterfaceName = syncInterface.Namespace + "." + syncInterface.Name + "Async";

			asyncInterface = module.DefineType(asyncInterfaceName,
			                                   TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);

			asyncInterface.SetCustomAttribute(CreateAttribute<ServiceContractAttribute>("Name", syncInterface.Name));
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

		static bool HasAttribute(MethodInfo method, Type attribute)
		{
			return method.GetCustomAttributes(attribute, true).Length > 0;
		}

		static CustomAttributeBuilder CreateAttribute<TAttribute>(string property, object value) where TAttribute : Attribute
		{
			Type attributeType = typeof(TAttribute);

			return new CustomAttributeBuilder(attributeType.GetConstructor(new Type[0]), new object[0],
											  new[] { attributeType.GetProperty(property) }, new[] { value });
		}
	}
}
