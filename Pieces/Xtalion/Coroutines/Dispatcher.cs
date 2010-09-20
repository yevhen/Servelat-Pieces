#region Author

//// Yevhen Bobrov, http://blog.xtalion.com 

#endregion

using System;

namespace Xtalion.Coroutines
{
	public interface IDispatcher
	{
		void Invoke(Action action);
	}

	public static class Dispatcher
	{
		static IDispatcher current = new DirectInvoker();

		public static IDispatcher Current
		{
			get { return current; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				current = value;
			}
		}
	}

	public class DirectInvoker : IDispatcher
	{
		public void Invoke(Action action)
		{
			action();
		}
	}
}
