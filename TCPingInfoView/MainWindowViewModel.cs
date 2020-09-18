using ReactiveUI;

namespace TCPingInfoView
{
	public class MainWindowViewModel : ReactiveObject
	{
		private readonly MainWindow _window;

		public MainWindowViewModel(MainWindow window)
		{
			_window = window;
		}
	}
}
