#region Author

//// Yevhen Bobrov, http://blog.xtalion.com 

#endregion

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using Xtalion.Async;

namespace Xtalion.Silverlight.Services
{
	public abstract class ServiceCall<TService> : AsyncCall
	{
		readonly ServiceChannelFactory<TService> factory;
		readonly MethodCallExpression expression;

		object channel;

		protected ServiceCall(ServiceChannelFactory<TService> factory, MethodCallExpression expression)
		{
			this.factory = factory;
			this.expression = expression;
		}
		
		public override void Execute()
		{
			channel = factory.CreateChannel();
			object[] parameters = BuildParameters();

			MethodInfo beginMethod = GetBeginMethod();
			beginMethod.Invoke(channel, parameters);
		}

		MethodInfo GetBeginMethod()
		{
			return GetMethod("Begin" + expression.Method.Name);
		}

		MethodInfo GetEndMethod()
		{
			return GetMethod("End" + expression.Method.Name);
		}

		MethodInfo GetMethod(string methodName)
		{
			return channel.GetType().GetMethod(methodName);
		}

		object[] BuildParameters()
		{
			var parameters = new List<object>();

			foreach (Expression argument in expression.Arguments)
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

		protected abstract void HandleResult(object result);
	}
}
