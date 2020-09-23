using Microsoft.Extensions.Logging;
using ReactiveUI;
using Syncfusion.SfSkinManager;
using TCPingInfoView.Interfaces;

namespace TCPingInfoView
{
	public partial class MainWindow
	{
		public MainWindow(ILogger<MainWindow> logger, IPluginLoader pluginLoader, ILocalize localize)
		{
			SfSkinManager.ApplyStylesOnApplication = true;
			InitializeComponent();
			ViewModel = new MainWindowViewModel(this, logger, pluginLoader, localize);

			this.WhenActivated(d =>
			{
				logger.LogDebug(@"MainWindow Activated");
			});
		}
	}
}
