using Microsoft.Extensions.Logging;
using ReactiveUI;
using TCPingInfoView.Interfaces;

namespace TCPingInfoView
{
	public partial class MainWindow
	{
		public MainWindow(ILogger<MainWindow> logger, IPluginLoader pluginLoader, ILocalize localize)
		{
			InitializeComponent();
			ViewModel = new MainWindowViewModel(this, logger, pluginLoader, localize);

			this.WhenActivated(d =>
			{
				logger.LogDebug(@"MainWindow Activated");
			});
		}
	}
}
