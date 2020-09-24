using Microsoft.Extensions.Logging;
using ReactiveUI;
using System.Reactive.Disposables;
using TCPingInfoView.Interfaces;
using TCPingInfoView.ViewModels;

namespace TCPingInfoView
{
	public partial class MainWindow
	{
		public MainWindow(ILogger<MainWindow> logger,
			IPluginLoader pluginLoader,
			ILocalize localize,
			IConfigService configService)
		{
			InitializeComponent();
			ViewModel = new MainWindowViewModel(this, logger, pluginLoader, localize, configService);

			this.WhenActivated(d =>
			{
				logger.LogDebug(@"MainWindow Activated");
				this.BindCommand(ViewModel, vm => vm.ShowWindowCommand, v => v.NotifyIcon, nameof(NotifyIcon.TrayLeftMouseUp)).DisposeWith(d);
			});
		}
	}
}
