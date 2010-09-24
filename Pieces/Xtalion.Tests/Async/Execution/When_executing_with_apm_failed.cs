using System;

using NUnit.Framework;
using Rhino.Mocks;

namespace Xtalion.Async.Execution
{
	[Context]
	public class When_executing_with_apm_failed : ContextSpecification
	{
		ArgumentException exception = new ArgumentException();

		AsyncCommand command;
		AsyncQuery<string> query;

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

			command = new AsyncCommand(() => instance.Method(null), true);
			query = new AsyncQuery<string>(() => instance.Function(null), true);
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
