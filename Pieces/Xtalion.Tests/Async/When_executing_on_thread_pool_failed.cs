using System;

using NUnit.Framework;
using Rhino.Mocks;

namespace Xtalion.Async
{
	[Context]
	public class When_executing_on_thread_pool_failed : ContextSpecification
	{
		readonly ArgumentException exception = new ArgumentException();

		AsyncCommand command;
		AsyncQuery<string> query;

		TestInterface instance;

		protected override void Given()
		{
			instance = MockRepository.GenerateStub<TestInterface>();

			instance.Expect(x => x.Method(null)).Throw(exception);
			instance.Expect(x => x.Function(null)).Throw(exception);

			command = new AsyncCommand(()=> instance.Method(null));
			query = new AsyncQuery<string>(()=> instance.Function(null));
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
			Assert.That(command.Exception, Is.SameAs(exception));

			Assert.That(query.Failed);
			Assert.That(query.Exception, Is.SameAs(exception));
		}

		[Then]
		public void Should_throw_when_returning_result()
		{
			Assert.Throws<InvalidOperationException>(()=> Console.Write(query.Result));
		}
	}
}
