using System;
using System.Linq;
using System.Linq.Expressions;

namespace Xtalion.Async
{
	class DirectInvocation : Invocation
	{
		readonly object target;
		readonly MethodCallExpression expression;

		public DirectInvocation(object target, MethodCallExpression expression)
		{
			this.target = target;
			this.expression = expression;
		}

		public override void Invoke()
		{
			object[] parameters = EvaluateParameters(expression).ToArray();

			try
			{
				Result = expression.Method.Invoke(target, parameters);
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
