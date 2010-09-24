using System.Threading;

using NUnit.Framework;
using Rhino.Mocks;

namespace Xtalion.Async.Execution
{
	[Context]
	public class When_executing_on_thread_pool : ContextSpecification
	{
		const string parameter = "param";
		const string result = "result";

		AsyncCommand command;
		AsyncQuery<string> query;

		TestInterface instance;

		int callThreadId;
		int currentThreadId = CurrentThreadId();

		protected override void Given()
		{
			instance = MockRepository.GenerateStrictMock<TestInterface>();

			instance.Expect(x => x.Method(parameter))
					.WhenCalled(x => callThreadId = CurrentThreadId());

			instance.Expect(x => x.Function(parameter)).Return(result)
					.WhenCalled(x => callThreadId = CurrentThreadId()); ;

			command = new AsyncCommand(()=> instance.Method(parameter));
			query = new AsyncQuery<string>(()=> instance.Function(parameter));
		}

		protected override void When()
		{
			Execute(command);
			Execute(query);
		}

		[Then]
		public void Should_correctly_call_synchronous_version()
		{
			instance.VerifyAllExpectations();
		}

		[Then]
		public void Should_execute_on_different_thread()
		{
			Assert.That(callThreadId, Is.Not.EqualTo(currentThreadId));
		}

		[Then]
		public void Should_handle_result()
		{
			Assert.That(query.Result, Is.EqualTo(result));
		}

		static int CurrentThreadId()
		{
			return Thread.CurrentThread.ManagedThreadId;
		}
	}
}
