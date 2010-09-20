#region Author

//// Rob Eisenberg, Blue Spire Consulting, Inc 

#endregion

using System;
using System.Collections.Generic;

namespace Xtalion.Coroutines
{
	public class Run
	{
		readonly IEnumerator<IAction> iterator;

		Run(IEnumerable<IAction> sequence)
		{
			iterator = sequence.GetEnumerator();
		}

		void Iterate()
		{
			ActionCompleted(null, EventArgs.Empty);
		}

		void ActionCompleted(object sender, EventArgs args)
		{
			var previous = sender as IAction;

			if (previous != null)
				previous.Completed -= ActionCompleted;

			if (!iterator.MoveNext())
				return;

			IAction next = iterator.Current;
			next.Completed += ActionCompleted;
			next.Execute();
		}

		public static void Sequence(IEnumerable<IAction> sequence)
		{
			new Run(sequence).Iterate();
		}
	}
}