using PingClientBase;
using System;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace TCPingInfoView.Views.Converters
{
	public class LatencyConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// Tooltip
			if (value == null && targetType == null)
			{
				return null;
			}

			if (value is PingResult result)
			{
				// Display
				if (targetType == typeof(string))
				{
					if (result.Latency < 0)
					{
						return null;
					}
					return result.Latency;
				}

				// Background
				if (targetType == typeof(Brush))
				{
					if (result.Status == IPStatus.TimedOut || result.Status == IPStatus.Unknown)
					{
						return new SolidColorBrush(Colors.Red);
					}

					if (result.Latency >= 0)
					{
						return new SolidColorBrush(Color.FromRgb(0, 255, 170));
					}
				}

				// Tooltip
				if (targetType == null)
				{
					if (result.Latency < 0)
					{
						return result.Status;
					}
					return $@"{result.Latency} ms";
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
