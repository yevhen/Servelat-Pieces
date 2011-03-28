using System;
using System.Collections.Generic;
using System.ServiceModel;
using NUnit.Framework;

namespace Xtalion.Silverlight.Services
{
	[TestFixture]
	public class AsyncServiceInterfaceFactoryKnownTypesFixture
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
			Assert.That(asyncAttribute.MethodName, Is.EqualTo(syncAttribute.MethodName));
			Assert.That(asyncAttribute.DeclaringType, Is.EqualTo(syncAttribute.DeclaringType));
		}

		[ServiceContract]
		[ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
		public interface ITestService
		{
			[OperationContract]
			IEnumerable<Animal> GetAnimals();
		}

		public class TestService : ITestService
		{
			public IEnumerable<Animal> GetAnimals()
			{
				return new Animal[] {new Dog(), new Cat()};
			}
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
