using System;
using System.ServiceModel;
using NUnit.Framework;

namespace Xtalion.Silverlight.Services.KnownTypes
{
	[TestFixture]
	public class ProvidedByEnumeratingTypesFixture
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
			Assert.That(asyncAttribute.Type, Is.Not.Null);
			Assert.That(asyncAttribute.MethodName, Is.Null);
			Assert.That(asyncAttribute.DeclaringType, Is.Null);
		}

		[Test]
		public void Should_copy_all_instances_of_ServiceKnownType_attribute()
		{
			var asyncAttributes = (ServiceKnownTypeAttribute[])asyncInterface.GetCustomAttributes(typeof(ServiceKnownTypeAttribute), true);

			Assert.That(asyncAttributes.Length, Is.EqualTo(2));
			Assert.That(asyncAttributes[0].Type, Is.EqualTo(typeof(Dog)));
			Assert.That(asyncAttributes[1].Type, Is.EqualTo(typeof(Cat)));
		}

		[ServiceContract]
		[ServiceKnownType(typeof(Dog))]
		[ServiceKnownType(typeof(Cat))]
		public interface ITestService
		{
			[OperationContract]
			void NoOp();
		}

		public abstract class Animal {}
		public class Dog : Animal {}
		public class Cat : Animal {}
	}
}