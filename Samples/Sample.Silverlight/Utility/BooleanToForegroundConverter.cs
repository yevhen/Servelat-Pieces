using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Sample.Silverlight.Utility
{
	public class BooleanToForegroundConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Brush)) 
				throw new InvalidOperationException("The target must be a Color!");

			var isTrue = (bool)value;

			return (isTrue ? new SolidColorBrush(Colors.Gray) : new SolidColorBrush(Colors.Black)); 
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return DependencyProperty.UnsetValue; 
		}
	}
}
