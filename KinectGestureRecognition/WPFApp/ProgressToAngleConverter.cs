using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPFApp
{
	public class ProgressToAngleConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			double progress = (double) values[0];
			ProgressBar progressBar = values[1] as ProgressBar;
			string type = (string) parameter;

			return 359.999*(progress/(progressBar.Maximum - progressBar.Minimum));
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
		                            CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}