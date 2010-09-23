using System;

using NUnit.Framework;

namespace Xtalion.Async.Execution
{
	[TestFixture]
	public class ApmExecutionMethodFixture : ExecutionMethodFixtureBase
	{
		[Test]
		public void Successfull_execution()
		{
			var call = new AsyncCallStub<string>();
			var target = new TestInterfaceMock {Result = "result"};

			var method = new ApmExecutionMethod<string>(call, AsMethodCallExpression(() => target.Function("param")));
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
			var target = new TestInterfaceMock {Result = "result", Exception = exception};

			var method = new ApmExecutionMethod<string>(call, AsMethodCallExpression(() => target.Function("param")));
			Execute(method, call);

			AssertExecutionOutcome(target);
			AssertExecutionFailedWith(exception, call);
			AssertExecutionResult(call, default(string));
		}

		static void AssertExecutionOutcome(TestInterfaceMock target)
		{
			Assert.That(!target.FunctionWasCalled);

			Assert.That(target.BeginFunctionWasCalled);
			Assert.That(target.EndFunctionWasCalled);

			Assert.That(target.ParameterPassedToBeginFunction, Is.EqualTo("param"));
			Assert.That(target.AsyncResultGiven, Is.SameAs(target.AsyncResultPassedBack));
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
			Assert.That(call.Exception.InnerException, Is.SameAs(exception));
		}
	}
}
