using System;
using System.Threading;

using Xtalion.Coroutines;

namespace Xtalion.Async
{
	public class ContextSpecification : Specification
	{
		public interface TestInterface
		{
			void Method(string param);
			string Function(string param);

			IAsyncResult BeginMethod(string param, AsyncCallback callback, object asyncState);
			void EndMethod(IAsyncResult result);

			IAsyncResult BeginFunction(string param, AsyncCallback callback, object asyncState);
			string EndFunction(IAsyncResult result);
		}

		protected void Execute(IAction action)
		{
			var signal = new ManualResetEvent(false);

			action.Completed += (sender, e) => signal.Set();
			action.Execute();

			signal.WaitOne();
		}
	}
}
