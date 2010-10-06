using System;

using NUnit.Framework;
using Rhino.Mocks;

namespace Xtalion.Async
{
	[Context]
	public class When_executing_with_apm_failed : ContextSpecification
	{
		readonly ArgumentException exception = new ArgumentException();

		AsyncPatternCommand command;
		AsyncPatternQuery<string> query;

		TestInterface instance;

		protected override void Given()
		{
			instance = MockRepository.GenerateStub<TestInterface>();

			instance.Expect(
				x => x.BeginMethod(
						Arg<string>.Is.Anything,
						Arg<AsyncCallback>.Is.Anything,
						Arg<object>.Is.Anything))
					.Return(null)
					.WhenCalled(x => ((AsyncCallback)x.Arguments[1])(null));

			instance.Expect(x => x.EndMethod(null))
					.Throw(exception);

			instance.Expect(
				x => x.BeginFunction(
						Arg<string>.Is.Anything,
						Arg<AsyncCallback>.Is.Anything,
						Arg<object>.Is.Anything))
					.Return(null)
					.WhenCalled(x => ((AsyncCallback)x.Arguments[1])(null));

			instance.Expect(x => x.EndFunction(null))
					.Throw(exception);

			command = new AsyncPatternCommand(() => instance.Method(null));
			query = new AsyncPatternQuery<string>(() => instance.Function(null));
		}

		protected override void When()
		{
			Execute(command);
			Execute(query);
		}

		[Then]
		public void Should_catch_any_exceptions_thrown()
		{
			Assert.That(command.Failed);
			Assert.That(command.Exception.InnerException, Is.SameAs(exception));

			Assert.That(query.Failed);
			Assert.That(query.Exception.InnerException, Is.SameAs(exception));
		}

		[Then]
		public void Should_throw_when_returning_result()
		{
			Assert.Throws<InvalidOperationException>(() => Console.Write(query.Result));
		}
	}
}
