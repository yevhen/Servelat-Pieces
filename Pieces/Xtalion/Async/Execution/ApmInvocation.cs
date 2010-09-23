using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Xtalion.Async.Execution
{
	internal class ApmInvocation
	{
		public EventHandler End;

		readonly MethodInfo beginMethod;
		readonly MethodInfo endMethod;

		readonly object target;
		readonly object[] parameters;

		public ApmInvocation(MethodCallExpression expression)
			: this(GetTarget(expression), expression)
		{}

		public ApmInvocation(object target, MethodCallExpression expression)
		{
			this.target = target;

			beginMethod = GetBeginMethod(expression);
			endMethod	= GetEndMethod(expression);
			
			parameters	= BuildParameters(expression);
		}
		
		static object GetTarget(MethodCallExpression expression)
		{
			return Expression.Lambda(expression.Object).Compile().DynamicInvoke();
		}

		MethodInfo GetBeginMethod(MethodCallExpression expression)
		{
			return GetMethod("Begin", expression.Method.Name);
		}

		MethodInfo GetEndMethod(MethodCallExpression expression)
		{
			return GetMethod("End", expression.Method.Name);
		}

		MethodInfo GetMethod(string prefix, string name)
		{
			return target.GetType().GetMethod(prefix + name);
		}

		object[] BuildParameters(MethodCallExpression expression)
		{
			var result = new List<object>();

			foreach (Expression argument in expression.Arguments)
			{
				object parameter = Expression.Lambda(argument).Compile().DynamicInvoke();
				result.Add(parameter);
			}

			result.Add(new AsyncCallback(OnInvokeCompleted));
			result.Add(null);

			return result.ToArray();
		}

		public void Invoke()
		{
			beginMethod.Invoke(target, parameters);
		}

		public Exception Exception 
		{ 
			get; private set; 
		}

		public object Result
		{
			get; private set;
		}

		void OnInvokeCompleted(IAsyncResult ar)
		{
			try
			{
				Result = endMethod.Invoke(target, new object[] {ar});
			}
			catch (Exception exc)
			{
				Exception = exc;
			}

			if (End != null)
				End(this, EventArgs.Empty);
		}
	}
}
