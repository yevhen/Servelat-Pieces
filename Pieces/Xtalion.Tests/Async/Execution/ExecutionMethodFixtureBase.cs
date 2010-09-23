using System;
using System.Linq.Expressions;
using System.Threading;

using Xtalion.Coroutines;

namespace Xtalion.Async.Execution
{
	public class ExecutionMethodFixtureBase
	{
		internal void Execute(ExecutionMethod method, IAction action)
		{
			var signal = new ManualResetEvent(false);
			
			action.Completed += (sender, e) => signal.Set();
			method.Execute();

			signal.WaitOne();
		}

		protected MethodCallExpression AsMethodCallExpression<TResult>(Expression<Func<TResult>> expression)
		{
			return (MethodCallExpression)expression.Body;
		}

		protected Func<TResult> AsFunc<TResult>(Expression<Func<TResult>> expression)
		{
			return expression.Compile();
		}

		public interface TestInterface
		{
			string Function(string param);

			IAsyncResult BeginFunction(string param, AsyncCallback callback, object asyncState);
			string EndFunction(IAsyncResult result);
		}

		#region Mocks

		public class TestInterfaceMock : TestInterface
		{
			public bool FunctionWasCalled;
			public int FunctionWasCalledOnThreadWithId;
			public string ParameterPassedToFunction;

			public string Result;
			public Exception Exception;

			public string Function(string param)
			{
				FunctionWasCalled = true;
				FunctionWasCalledOnThreadWithId = Thread.CurrentThread.ManagedThreadId;
				ParameterPassedToFunction = param;

				if (Exception != null)
					throw Exception;

				return Result;
			}

			public bool EndFunctionWasCalled;
			public bool BeginFunctionWasCalled;
			public string ParameterPassedToBeginFunction;

			public IAsyncResult AsyncResultGiven;
			public IAsyncResult AsyncResultPassedBack;

			public IAsyncResult BeginFunction(string param, AsyncCallback callback, object asyncState)
			{
				BeginFunctionWasCalled = true;
				ParameterPassedToBeginFunction = param;

				AsyncResultGiven = new AsyncResultStub();
				callback(AsyncResultGiven);

				return null; // doesn't matter here
			}

			public string EndFunction(IAsyncResult result)
			{
				EndFunctionWasCalled = true;
				AsyncResultPassedBack = result;

				if (Exception != null)
					throw Exception;

				return Result;
			}
		}

		#endregion
		
		#region Stubs

		protected class AsyncCallStub : AsyncCall
		{
			public override void Execute()
			{
				throw new NotImplementedException();
			}
		}

		protected class AsyncCallStub<TResult> : AsyncCall<TResult>
		{
			public override void Execute()
			{
				throw new NotImplementedException();
			}
		}

		protected class AsyncResultStub : IAsyncResult
		{
			public bool IsCompleted
			{
				get { return true; }
			}

			public WaitHandle AsyncWaitHandle
			{
				get { return null; }
			}

			public object AsyncState
			{
				get { return null; }
			}

			public bool CompletedSynchronously
			{
				get { return false; }
			}
		}

		#endregion
	}
}
