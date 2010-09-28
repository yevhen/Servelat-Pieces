#region Author

//// Rob Eisenberg, Blue Spire Consulting, Inc 

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Input;

using Xtalion.Coroutines;

namespace Xtalion.Silverlight
{
	public class ViewModelCommand : ICommand
	{
		readonly MethodInfo execute;
		readonly PropertyInfo canExecute;
		readonly ViewModelBase viewModel;

		public event EventHandler CanExecuteChanged = delegate { };

		public ViewModelCommand(ViewModelBase viewModel, MethodInfo execute, PropertyInfo canExecute)
		{
			this.viewModel = viewModel;
			this.execute = execute;
			this.canExecute = canExecute;

			ObserveCanExecuteChanged();
		}

		void ObserveCanExecuteChanged()
		{
			if (canExecute == null)
				return;

			viewModel.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == canExecute.Name)
					CanExecuteChanged(this, EventArgs.Empty);
			};
		}

		public bool CanExecute(object parameter)
		{
			return canExecute == null || (bool) canExecute.GetValue(viewModel, null);
		}

		public void Execute(object parameter)
		{
			object returnValue = execute.Invoke(viewModel, null);

			if (returnValue != null)
				HandleReturnValue(returnValue);
		}

		private static void HandleReturnValue(object returnValue)
		{
			var routine = returnValue as IEnumerable<IAction>;

			if (routine != null)
				Yield.Routine((routine));
		}
	}
}