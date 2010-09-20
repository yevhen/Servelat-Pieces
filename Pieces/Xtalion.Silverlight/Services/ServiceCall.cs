#region Author

//// Yevhen Bobrov, http://blog.xtalion.com 

#endregion

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using Xtalion.Coroutines;
using Xtalion.Silverlight;

namespace Xtalion.Silverlight.Services
{
	public abstract class ServiceCall<TService> : IAction
	{
		public event EventHandler Completed;

		readonly ServiceChannelFactory<TService> factory;
		readonly MethodCallExpression call;

		object channel;

		protected ServiceCall(ServiceChannelFactory<TService> factory, MethodCallExpression call)
		{
			this.factory = factory;
			this.call = call;
		}
		
		public bool Failed
		{
			get { return Exception != null; }
		}

		public Exception Exception
		{
			get; private set;
		}

		public void Execute()
		{
			channel = factory.CreateChannel();
			object[] parameters = BuildParameters();

			MethodInfo beginMethod = GetBeginMethod();
			beginMethod.Invoke(channel, parameters);
		}

		MethodInfo GetBeginMethod()
		{
			return GetMethod("Begin" + call.Method.Name);
		}

		MethodInfo GetEndMethod()
		{
			return GetMethod("End" + call.Method.Name);
		}

		MethodInfo GetMethod(string methodName)
		{
			return channel.GetType().GetMethod(methodName);
		}

		object[] BuildParameters()
		{
			var parameters = new List<object>();

			foreach (Expression argument in call.Arguments)
			{
				object parameter = Expression.Lambda(argument).Compile().DynamicInvoke();
				parameters.Add(parameter);
			}

			parameters.Add(new AsyncCallback(OnCallCompleted));
			parameters.Add(null);

			return parameters.ToArray();
		}

		void OnCallCompleted(IAsyncResult ar)
		{
			try
			{
				MethodInfo endMethod = GetEndMethod();
				HandleResult(endMethod.Invoke(channel, new object[]{ar}));
			}
			catch (Exception exc)
			{
				Exception = exc;
			}

			SignalCompleted();
		}

		void SignalCompleted()
		{
			EventHandler handler = Completed;

			if (handler != null)
				Call.OnUIThread(() => handler(this, EventArgs.Empty));			
		}

		protected abstract void HandleResult(object result);
	}
}
