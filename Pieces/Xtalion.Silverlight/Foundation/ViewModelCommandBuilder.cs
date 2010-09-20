#region Author

//// Yevhen Bobrov, http://blog.xtalion.com 

#endregion

using System;
using System.Linq.Expressions;
using System.Windows.Input;

namespace Xtalion.Silverlight
{
	public class ViewModelCommandBuilder
	{
		readonly ViewModelBase viewModel;

		public ViewModelCommandBuilder(ViewModelBase viewModel)
		{
			this.viewModel = viewModel;
		}

		public ICommand For(Expression<Action> expression)
		{
			var methodCall = (MethodCallExpression)expression.Body;

			return new ViewModelCommand(viewModel, methodCall.Method, 
										viewModel.GetType().GetProperty("Can" + methodCall.Method.Name));
		}
	}
}