using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using NUnit.Framework;

namespace Xtalion.Silverlight.Services.KnownTypes
{
	[TestFixture]
	public class ProvidedByAStaticClassFixture
	{
		Type syncInterface;
		Type asyncInterface;

		[SetUp]
		public void SetUp()
		{
			syncInterface = typeof(ITestService);

			var factory = new AsyncServiceInterfaceFactory();
			asyncInterface = factory.Generate(syncInterface);
		}

		[Test]
		public void Should_copy_ServiceKnownTypeAttribute_to_async_interface()
		{
			Assert.That(asyncInterface, Has.Attribute<ServiceKnownTypeAttribute>());
		}

		[Test]
		public void Should_copy_all_properties_of_ServiceKnownTypeAttribute_to_async_interface()
		{
			var syncAttribute = (ServiceKnownTypeAttribute)syncInterface.GetCustomAttributes(typeof(ServiceKnownTypeAttribute), true)[0];
			var asyncAttribute = (ServiceKnownTypeAttribute)asyncInterface.GetCustomAttributes(typeof(ServiceKnownTypeAttribute), true)[0];

			Assert.That(asyncAttribute.Type, Is.EqualTo(syncAttribute.Type));
			Assert.That(asyncAttribute.Type, Is.Null);
			Assert.That(asyncAttribute.MethodName, Is.EqualTo(syncAttribute.MethodName));
			Assert.That(asyncAttribute.DeclaringType, Is.EqualTo(syncAttribute.DeclaringType));
		}

		[Test]
		public void Should_ensure_that_static_provider_class_with_method_is_reachable()
		{
			var asyncAttribute = (ServiceKnownTypeAttribute)asyncInterface.GetCustomAttributes(typeof(ServiceKnownTypeAttribute), true)[0];
			
			var methodInfo = asyncAttribute.DeclaringType.GetMethod(asyncAttribute.MethodName);
			var result = methodInfo.Invoke(null, null);
			var resultArray = result as Type[];
			Assert.IsNotNull(resultArray);
			Assert.That(resultArray.Length, Is.EqualTo(2));
			Assert.That(resultArray[0], Is.EqualTo(typeof(Dog)));
			Assert.That(resultArray[1], Is.EqualTo(typeof(Cat)));
		}

		[ServiceContract]
		[ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
		public interface ITestService
		{
			[OperationContract]
			IEnumerable<Animal> GetAnimals();
		}

		public abstract class Animal
		{
			public abstract string Name { get; set; }
		}

		public class Dog : Animal
		{
			public override string Name { get; set; }
		}

		public class Cat : Animal
		{
			public override string Name { get; set; }
		}

		public static class KnownTypesProvider
		{
			public static IEnumerable<Type> GetKnownTypes()
			{
				return new[] {typeof (Dog), typeof (Cat)};
			}
		}
	}
}
