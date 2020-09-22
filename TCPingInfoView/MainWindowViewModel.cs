﻿using Microsoft.Extensions.Logging;
using ReactiveUI;
using System;
using System.Threading.Tasks;
using TCPingInfoView.Interfaces;
using TCPingInfoView.Utils;

namespace TCPingInfoView
{
	public class MainWindowViewModel : ReactiveObject
	{
		private readonly MainWindow _window;
		private readonly ILogger _logger;
		private readonly IPluginLoader _pluginLoader;

		public MainWindowViewModel(MainWindow window, ILogger logger, IPluginLoader pluginLoader)
		{
			_window = window;
			_logger = logger;
			_pluginLoader = pluginLoader;

			ReloadDefaultPlugins().NoWarning();
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

		public void ShowWindow()
		{
			_window.ShowWindow();
		}
	}
}
