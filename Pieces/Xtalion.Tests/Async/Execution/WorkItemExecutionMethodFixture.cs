using System;
using System.Threading;

using NUnit.Framework;

namespace Xtalion.Async.Execution
{
	[TestFixture]
	public class WorkItemExecutionMethodFixture : ExecutionMethodFixtureBase
	{
		int currentThreadId;

		[SetUp]
		public void SetUp()
		{
			currentThreadId = Thread.CurrentThread.ManagedThreadId;
		}
		
		[Test]
		public void Successfull_execution()
		{
			var call = new AsyncCallStub<string>();
			var target = new TestInterfaceMock { Result = "result" };

			var method = new WorkItemExecutionMethod<string>(call, AsFunc(() => target.Function("param")));
			Execute(method, call);

			AssertExecutionOutcome(target);
			AssertExecutionWasSuccessfull(call);
			AssertExecutionResult(call, "result");
		}

		[Test]
		public void Failed_execution()
		{
			var exception = new ArgumentException();

			var call = new AsyncCallStub<string>();
			var target = new TestInterfaceMock { Result = "result", Exception = exception };

			var method = new WorkItemExecutionMethod<string>(call, AsFunc(() => target.Function("param")));
			Execute(method, call);

			AssertExecutionOutcome(target);
			AssertExecutionFailedWith(exception, call);
			AssertExecutionResult(call, default(string));
		}

		void AssertExecutionOutcome(TestInterfaceMock target)
		{
			Assert.That(!target.BeginFunctionWasCalled);
			Assert.That(!target.EndFunctionWasCalled);

			Assert.That(target.FunctionWasCalled);
			Assert.That(target.ParameterPassedToFunction, Is.EqualTo("param"));

			Assert.That(target.FunctionWasCalledOnThreadWithId, Is.Not.EqualTo(currentThreadId));
		}

		static void AssertExecutionResult(AsyncCall<string> call, string result)
		{
			Assert.That(call.Result, Is.EqualTo(result));
		}

		static void AssertExecutionWasSuccessfull(AsyncCall call)
		{
			Assert.That(!call.Failed);
		}

		static void AssertExecutionFailedWith(ArgumentException exception, AsyncCall call)
		{
			Assert.That(call.Failed);
			Assert.That(call.Exception, Is.SameAs(exception));
		}
	}
}
