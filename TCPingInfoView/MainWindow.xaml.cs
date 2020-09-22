using Microsoft.Extensions.Logging;
using ReactiveUI;
using TCPingInfoView.Interfaces;

namespace TCPingInfoView
{
	public partial class MainWindow
	{
		public MainWindow(ILogger<MainWindow> logger, IPluginLoader pluginLoader)
		{
			InitializeComponent();
			ViewModel = new MainWindowViewModel(this, logger, pluginLoader);

			this.WhenActivated(d =>
			{
				logger.LogDebug(@"MainWindow Activated");
			});
		}
	}
}
