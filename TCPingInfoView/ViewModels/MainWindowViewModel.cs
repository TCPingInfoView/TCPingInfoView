using DynamicData;
using Microsoft.Extensions.Logging;
using PingClientBase;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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
		private readonly IDnsQuery _dnsQuery;

		#region Command

		public ReactiveCommand<Unit, Unit> ShowWindowCommand { get; }
		public ReactiveCommand<Unit, Unit> HideWindowCommand { get; }
		public ReactiveCommand<Unit, Unit> ExitCommand { get; }
		public ReactiveCommand<Unit, Unit> TestCommand { get; }

		#endregion

		private SourceList<Server> ServerSourceList { get; } = new SourceList<Server>();
		public readonly ReadOnlyObservableCollection<Server> ServerList;

		public MainWindowViewModel(MainWindow window,
			ILogger logger,
			IPluginLoader pluginLoader,
			ILocalize localize,
			IConfigService configService,
			IDnsQuery dnsQuery)
		{
			_window = window;
			_logger = logger;
			_pluginLoader = pluginLoader;
			_localize = localize;
			ConfigService = configService;
			_dnsQuery = dnsQuery;

			ServerSourceList
					.Connect()
					.ObserveOnDispatcher()
					.Bind(out ServerList)
					.DisposeMany()
					.Subscribe();

			InitAsync().NoWarning();

			ShowWindowCommand = ReactiveCommand.Create(ShowWindow);
			HideWindowCommand = ReactiveCommand.Create(HideWindow);
			ExitCommand = ReactiveCommand.Create(Exit);
			TestCommand = ReactiveCommand.CreateFromObservable(Test);
			TestCommand.ThrownExceptions.Subscribe(ex =>
			{
				_logger.LogError(ex, @"Test Error");
			});
		}

		private async Task InitAsync()
		{
			await ReloadDefaultPluginsAsync();
			await ConfigService.LoadAsync(default);
#if DEBUG
			ConfigService.Config.Servers.Clear();
			ConfigService.Config.Servers.Add(new Server
			{
				Hostname = @"localhost",
				Port = 3389,
				Protocol = @"TCP",
				Remark = @"本机 RDP",
			});
			ConfigService.Config.Servers.Add(new Server
			{
				Hostname = @"localhost",
				Port = 0,
				Protocol = @"TCP",
				Remark = @"端口错误",
			});
			ConfigService.Config.Servers.Add(new Server
			{
				Hostname = @"localhost",
				Port = 114,
				Protocol = @"TCP",
				Remark = @"测试失败",
			});
			ConfigService.Config.Servers.Add(new Server
			{
				Hostname = @"localhost",
				Port = 6666,
				Protocol = @"UDP",
				Remark = @"不支持的协议",
			});
			ConfigService.Config.Servers.Add(new Server
			{
				Hostname = @"a.b.c.d",
				Port = 3389,
				Protocol = @"TCP",
				Remark = @"IP 解析错误",
			});
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

		private void HideWindow()
		{
			_window.Hide();
		}

		private void Exit()
		{
			_window.CloseReason = CloseReason.ApplicationExitCall;
			_window.Close();
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

		private async Task TestServerAsync(Server server, CancellationToken token)
		{
			try
			{
				if (_pluginLoader.Plugins.TryGetValue(server.Protocol, out var func))
				{
					var client = func();
					client.Timeout = TimeSpan.FromSeconds(3);
					server.Ip = await _dnsQuery.QueryAsync(server.Hostname);
					if (server.Ip != null)
					{
						client.EndPoint = new IPEndPoint(server.Ip, server.Port);
						server.CurrentResult = await client.PingAsync(token);
					}
					else
					{
						// IP 解析错误
						server.CurrentResult = new PingResult
						{
							Latency = -1.0,
							Status = IPStatus.BadDestination,
							Info = @"Failed to resolve IP address!" //TODO: I18N
						};
					}
				}
				else
				{
					// 不支持的协议类型
					server.CurrentResult = new PingResult
					{
						Latency = -1.0,
						Status = IPStatus.DestinationProtocolUnreachable,
						Info = @"Protocol not supported!" //TODO: I18N
					};
				}
			}
			catch (Exception ex)
			{
				server.CurrentResult = new PingResult
				{
					Latency = -1.0,
					Status = IPStatus.Unknown,
					Info = $@"{ex}"
				};
			}
		}

		private IObservable<Unit> Test()
		{
			return ServerSourceList.Items
					.Select(server => Observable.FromAsync(ct => TestServerAsync(server, ct)))
					.Merge();
		}
	}
}
