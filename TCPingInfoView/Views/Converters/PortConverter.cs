using PingClientBase;
using System;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace TCPingInfoView.Views.Converters
{
	public class PortConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is PingResult result && targetType == typeof(Brush))
			{
				if (result.Status == IPStatus.DestinationPortUnreachable)
				{
					return new SolidColorBrush(Colors.Red);
				}
			}

			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}