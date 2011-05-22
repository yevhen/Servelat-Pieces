using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Xtalion.Async
{
	abstract class Invocation
	{
		public EventHandler Completed;

		public abstract void Invoke();

		public Exception Exception 
		{ 
			get; protected set; 
		}

		public object Result
		{
			get; protected set;
		}

		protected IEnumerable<object > EvaluateParameters(MethodCallExpression expression)
		{
			return expression.Arguments.Select(arg => Expression.Lambda(arg).Compile().DynamicInvoke());
		}
	}
}
