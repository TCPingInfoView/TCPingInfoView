using ReactiveUI;

namespace TCPingInfoView
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			ViewModel = new MainWindowViewModel(this);

			this.WhenActivated(d =>
			{

			});
		}
	}
}
