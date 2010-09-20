#region Author

 //// Rob Eisenberg, Blue Spire Consulting, Inc 

#endregion

using System;

namespace Xtalion.Coroutines
{
	public interface IAction
	{
		void Execute();
		event EventHandler Completed;
	}
}