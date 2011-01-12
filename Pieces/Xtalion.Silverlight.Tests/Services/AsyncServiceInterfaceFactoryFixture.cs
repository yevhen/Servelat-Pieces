using System;
using System.Reflection;
using System.ServiceModel;

using NUnit.Framework;

namespace Xtalion.Silverlight.Services
{
	[TestFixture]
	public class AsyncServiceInterfaceFactoryFixture
	{
		Type syncInterface;
		Type asyncInterface;

		[SetUp]
		public void SetUp()
		{
			syncInterface = typeof(ITestInterface);

			var factory = new AsyncServiceInterfaceFactory();
			asyncInterface = factory.Generate(syncInterface);
		}

		[Test]
		public void Should_create_async_interface_with_the_same_name_and_async_suffix()
		{
			Assert.That(asyncInterface.IsInterface, Is.True);
			Assert.That(asyncInterface.Name, Is.EqualTo(syncInterface.Name + "Async"));
			Assert.That(asyncInterface.Namespace, Is.EqualTo(syncInterface.Namespace));
			Assert.That(asyncInterface.Assembly.GetName().Name, Is.EqualTo("Xtalion.Silverlight.Services.Auto"));
		}

		[Test]
		public void Should_properly_generate_service_contract_attribute()
		{
			var syncAttribute = (ServiceContractAttribute)syncInterface.GetCustomAttributes(typeof(ServiceContractAttribute), true)[0];
			var asyncAttribute = (ServiceContractAttribute) asyncInterface.GetCustomAttributes(typeof(ServiceContractAttribute), true)[0];
			
			Assert.That(asyncAttribute.Name, Is.EqualTo(syncInterface.Name));
			Assert.That(asyncAttribute.Namespace, Is.EqualTo(syncAttribute.Namespace));
		}

		[Test]
		public void Should_create_async_method_pair_per_each_sync_method()
		{
			const int methodsWithOperationContractAttributeCount = 6;
			const int asyncMethodsPerSingleSyncMethodCount = 2;

			Assert.That(asyncInterface.GetMethods().Length, 
				Is.EqualTo(methodsWithOperationContractAttributeCount * asyncMethodsPerSingleSyncMethodCount));
		}

		[Test]
		public void Should_properly_generate_void_method_pair()
		{
			MethodInfo beginMethod = asyncInterface.GetMethod("BeginVoidMethod");
			MethodInfo endMethod = asyncInterface.GetMethod("EndVoidMethod");

			Assert.That(beginMethod.GetParameters().Length, Is.EqualTo(2));

			AssertBeginMethodRules(beginMethod, 0, 1);
			AssertEndMethodRules(endMethod, typeof(void));
		}

		[Test]
		public void Should_properly_generate_void_method_withparams_pair()
		{
			MethodInfo beginMethod = asyncInterface.GetMethod("BeginVoidMethodWithParams");
			MethodInfo endMethod = asyncInterface.GetMethod("EndVoidMethodWithParams");

			Assert.That(beginMethod.GetParameters().Length, Is.EqualTo(4));
			Assert.That(beginMethod.GetParameters()[0].ParameterType, Is.SameAs(typeof(int)));
			Assert.That(beginMethod.GetParameters()[1].ParameterType, Is.SameAs(typeof(string)));

			AssertBeginMethodRules(beginMethod, 2, 3);
			AssertEndMethodRules(endMethod, typeof(void));
		}

		[Test]
		public void Should_properly_generate_func_method_pair()
		{
			MethodInfo beginMethod = asyncInterface.GetMethod("BeginFunc");
			MethodInfo endMethod = asyncInterface.GetMethod("EndFunc");

			Assert.That(beginMethod.GetParameters().Length, Is.EqualTo(2));

			AssertBeginMethodRules(beginMethod, 0, 1);
			AssertEndMethodRules(endMethod, typeof(int));
		}		
		
		[Test]
		public void Should_properly_generate_func_method_withparams_pair()
		{
			MethodInfo beginMethod = asyncInterface.GetMethod("BeginFuncWithParams");
			MethodInfo endMethod = asyncInterface.GetMethod("EndFuncWithParams");

			Assert.That(beginMethod.GetParameters().Length, Is.EqualTo(4));
			Assert.That(beginMethod.GetParameters()[0].ParameterType, Is.SameAs(typeof(int)));
			Assert.That(beginMethod.GetParameters()[1].ParameterType, Is.SameAs(typeof(string)));

			AssertBeginMethodRules(beginMethod, 2, 3);
			AssertEndMethodRules(endMethod, typeof(int));
		}

		[Test]
		public void Should_copy_fault_contract_attribute()
		{
			MethodInfo syncMethod = syncInterface.GetMethod("OperationWithFaultContract");
			MethodInfo asyncMethod = asyncInterface.GetMethod("BeginOperationWithFaultContract");

			Assert.That(HasAttribute(asyncMethod, typeof(FaultContractAttribute)), Is.True);

			var originalAttribute = (FaultContractAttribute)syncMethod.GetCustomAttributes(typeof(FaultContractAttribute), true)[0];
			var copiedAttribute = (FaultContractAttribute)asyncMethod.GetCustomAttributes(typeof(FaultContractAttribute), true)[0];

			Assert.That(originalAttribute.DetailType, Is.EqualTo(copiedAttribute.DetailType));
		}

		[Test]
		public void Should_cache_generate_interface()
		{
			var factory = new AsyncServiceInterfaceFactory();

			var type1 = factory.Generate(typeof (ITestInterface));
			var type2 = factory.Generate(typeof (ITestInterface));

			Assert.That(type1, Is.SameAs(type2));
		}

		static void AssertBeginMethodRules(MethodInfo beginMethod, int firstRequiredParameterIndex, int secondRequiredParameterIndex)
		{
			Assert.That(beginMethod.ReturnType, Is.SameAs(typeof(IAsyncResult)));

			Assert.That(beginMethod.GetParameters()[firstRequiredParameterIndex].ParameterType, Is.SameAs(typeof(AsyncCallback)));
			Assert.That(beginMethod.GetParameters()[secondRequiredParameterIndex].ParameterType, Is.SameAs(typeof(object)));

			var attribute = (OperationContractAttribute)beginMethod.GetCustomAttributes(typeof(OperationContractAttribute), true)[0];
			Assert.That(attribute.AsyncPattern, Is.True);
		}

		static void AssertEndMethodRules(MethodInfo endMethod, Type returnType)
		{
			Assert.That(endMethod.ReturnType, Is.SameAs(returnType));
			Assert.That(endMethod.GetParameters().Length, Is.EqualTo(1));
			Assert.That(endMethod.GetParameters()[0].ParameterType, Is.SameAs(typeof(IAsyncResult)));
		}

		static bool HasAttribute(MethodInfo method, Type attribute)
		{
			return method.GetCustomAttributes(attribute, true).Length > 0;
		}
	}

	[ServiceContract(Namespace = "http://somenamespace")]
	[Whatever]
	public interface ITestInterface
	{
		[OperationContract]
		void VoidMethod();

		[OperationContract]
		void VoidMethodWithParams(int param1, string param2);

		[OperationContract]
		int Func();

		[OperationContract]
		int FuncWithParams(int param1, string param2);

		[OperationContract]
		[FaultContract(typeof(FaultReason))]
		void OperationWithFaultContract();

		[OperationContract]
		[Whatever]
		void OperationWithIgnoredAttributes();

		void NotAnOperation();
	}

	public class WhateverAttribute : Attribute
	{}
}
