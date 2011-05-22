using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Xtalion.Async
{
	class ApmInvocation : Invocation
	{
		readonly MethodInfo beginMethod;
		readonly MethodInfo endMethod;

		readonly object target;
		readonly object[] parameters;

		public ApmInvocation(object target, MethodCallExpression expression)
		{
			this.target = target;

			beginMethod = GetBeginMethod(expression);
			endMethod	= GetEndMethod(expression);
			
			parameters	= BuildParameters(expression);
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
			var result = EvaluateParameters(expression).ToList();

			result.Add(new AsyncCallback(OnInvokeCompleted));
			result.Add(null);

			return result.ToArray();
		}

		public override void Invoke()
		{
			beginMethod.Invoke(target, parameters);
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

			if (Completed != null)
				Completed(this, EventArgs.Empty);
		}
	}
}
