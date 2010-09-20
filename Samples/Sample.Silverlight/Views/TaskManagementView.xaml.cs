using System.Windows;
using System.Windows.Controls;

using Xtalion.Coroutines;

namespace Sample.Silverlight.Views
{
	public partial class TaskManagementView
	{
		public TaskManagementView()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			var viewModel = new TaskManagementViewModel();
			DataContext = viewModel;

			Run.Sequence(viewModel.Activate());
		}

		private void DescriptionTextChanged(object sender, TextChangedEventArgs e)
		{
			((TextBox) sender).GetBindingExpression(TextBox.TextProperty).UpdateSource();			
		}
	}
}