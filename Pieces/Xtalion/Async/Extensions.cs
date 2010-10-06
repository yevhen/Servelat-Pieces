using System.Linq.Expressions;

namespace Xtalion.Async
{
	public static class Extensions
	{
		public static object GetTarget(this MethodCallExpression expression)
		{
			return Expression.Lambda(expression.Object).Compile().DynamicInvoke();
		}
	}
}
