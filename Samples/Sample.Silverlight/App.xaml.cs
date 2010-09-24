using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Browser;

using Sample.Silverlight.Views;

namespace Sample.Silverlight
{
	public partial class App
	{
		public App()
		{
			Startup += Application_Startup;
			UnhandledException += Application_UnhandledException;

			InitializeComponent();
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			RootVisual = new TaskManagementView();
		}

		private static void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			ReportErrorToDOM(e);
			e.Handled = true;

			if (!Debugger.IsAttached)
			{
				e.Handled = true;
				Deployment.Current.Dispatcher.BeginInvoke(() => ReportErrorToDOM(e));
			}
		}

		private static void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
		{
			try
			{
				string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
				errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

				HtmlPage.Window.Eval(
				                     "throw new Error(\"Unhandled Error in Silverlight Application " +
				                     errorMsg + "\");");
			}
			catch (Exception)
			{}
		}
	}
}