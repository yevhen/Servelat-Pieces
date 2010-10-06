using System;
using System.Linq.Expressions;
using System.Threading;

namespace Xtalion.Async
{
	public class AsyncCommand : AsyncCall
	{
		readonly Action call;

		public AsyncCommand(Expression<Action> expression)
		{
			call = expression.Compile();			
		}

		public override void Execute()
		{
			ThreadPool.QueueUserWorkItem(ExecuteCall, null);
		}

		void ExecuteCall(object state)
		{
			try
			{
				call();
			}
			catch (Exception exc)
			{
				Exception = exc;
			}

			SignalCompleted();
		}
	}
}
