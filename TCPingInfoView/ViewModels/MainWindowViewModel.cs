using Microsoft.Extensions.Logging;
using ReactiveUI;
using System;
using System.Globalization;
using System.Reactive;
using System.Threading.Tasks;
using TCPingInfoView.Interfaces;
using TCPingInfoView.Utils;

namespace TCPingInfoView.ViewModels
{
	public class MainWindowViewModel : ReactiveObject
	{
		private readonly MainWindow _window;
		private readonly ILogger _logger;
		private readonly IPluginLoader _pluginLoader;
		private readonly ILocalize _localize;

		#region Command

		public ReactiveCommand<Unit, Unit> ShowWindowCommand { get; }

		#endregion

		public MainWindowViewModel(MainWindow window, ILogger logger, IPluginLoader pluginLoader, ILocalize localize)
		{
			_window = window;
			_logger = logger;
			_pluginLoader = pluginLoader;
			_localize = localize;

			ReloadDefaultPlugins().NoWarning();

			ShowWindowCommand = ReactiveCommand.Create(ShowWindow);
		}

		private async Task ReloadDefaultPlugins()
		{
			_logger.LogInformation(@"Loading plugins");
			try
			{
				await _pluginLoader.Clear();
				await _pluginLoader.LoadPluginsAsync(@"plugins");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, @"Load plugins Error");
			}
		}

		private void ShowWindow()
		{
			_window.ShowWindow();
		}

		public void ChangeLanguage(CultureInfo culture)
		{
			_localize.Current = culture;
		}
	}
}
