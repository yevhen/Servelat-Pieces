#region Author

//// Yevhen Bobrov, http://blog.xtalion.com 

#endregion

using System.Threading;

namespace Xtalion.Coroutines
{
	public static class Parallel
	{
		public static IAction Actions(params IAction[] actions)
		{
			return new ParallelAction(actions);
		}

		private class ParallelAction : DispatchAction
		{
			readonly IAction[] actions;
			int remaining;

			public ParallelAction(IAction[] actions)
			{
				this.actions = actions;
			}

			public override void Execute()
			{
				remaining = actions.Length;

				foreach (IAction action in actions)
				{
					action.Completed += delegate
					{
						if (Interlocked.Decrement(ref remaining) == 0)
							SignalCompleted();
					};

					action.Execute();
				}
			}
		}
	}
}
