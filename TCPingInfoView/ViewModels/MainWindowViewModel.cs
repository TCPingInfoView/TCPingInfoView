using DynamicData;
using Microsoft.Extensions.Logging;
using PingClientBase;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using TCPingInfoView.Interfaces;
using TCPingInfoView.Models;
using TCPingInfoView.Utils;

namespace TCPingInfoView.ViewModels
{
	public class MainWindowViewModel : ReactiveObject
	{
		private readonly MainWindow _window;
		private readonly ILogger _logger;
		private readonly IPluginLoader _pluginLoader;
		private readonly ILocalize _localize;
		public readonly IConfigService ConfigService;

		#region Command

		public ReactiveCommand<Unit, Unit> ShowWindowCommand { get; }

		#endregion

		private SourceList<Server> ServerSourceList { get; } = new SourceList<Server>();
		public readonly ReadOnlyObservableCollection<Server> ServerList;

		public MainWindowViewModel(MainWindow window,
			ILogger logger,
			IPluginLoader pluginLoader,
			ILocalize localize,
			IConfigService configService)
		{
			_window = window;
			_logger = logger;
			_pluginLoader = pluginLoader;
			_localize = localize;
			ConfigService = configService;

			ServerSourceList
					.Connect()
					.ObserveOnDispatcher()
					.Bind(out ServerList)
					.DisposeMany()
					.Subscribe();

			InitAsync().NoWarning();

			ShowWindowCommand = ReactiveCommand.Create(ShowWindow);
		}

		private async Task InitAsync()
		{
			await ReloadDefaultPluginsAsync();
			await ConfigService.LoadAsync(default);
#if DEBUG
			var server = new Server
			{
				Hostname = @"localhost",
				Port = 3389,
				Protocol = @"TCP",
				Remark = @"本机 RDP",
				Ip = IPAddress.IPv6Loopback,
				CurrentResult = new PingResult { Latency = 114514, Status = IPStatus.Success, Info = @"Test Info" }
			};
			while (ConfigService.Config.Servers.Count < 100)
			{
				ConfigService.Config.Servers.Add(server);
				ConfigService.Config.Servers.Add(server);
				ConfigService.Config.Servers.Add(server);
				ConfigService.Config.Servers.Add(server);
				ConfigService.Config.Servers.Add(server);
			}
			ConfigService.Config.Servers[0] = server;
#endif
			ReloadServersList();
		}

		private async Task ReloadDefaultPluginsAsync()
		{
			_logger.LogInformation(@"Loading plugins");
			try
			{
				await _pluginLoader.ClearAsync();
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

		private void ReloadServersList()
		{
			ResetListIndex();
			ServerSourceList.Clear();
			ServerSourceList.AddRange(ConfigService.Config.Servers);
		}

		private void ResetListIndex()
		{
			var servers = ConfigService.Config.Servers;
			for (var i = 0; i < servers.Count; ++i)
			{
				servers[i].Index = i + 1;
			}
		}
	}
}
