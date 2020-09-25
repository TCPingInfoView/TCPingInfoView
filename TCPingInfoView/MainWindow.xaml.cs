using Microsoft.Extensions.Logging;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
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
				this.BindCommand(ViewModel, vm => vm.ShowWindowCommand, v => v.NotifyIcon, nameof(NotifyIcon.TrayLeftMouseUp)).DisposeWith(d);
				this.BindCommand(ViewModel, vm => vm.ShowWindowCommand, v => v.ShowMenuItem).DisposeWith(d);

				this.BindCommand(ViewModel, vm => vm.HideWindowCommand, v => v.MinimizeButton).DisposeWith(d);

				this.BindCommand(ViewModel, vm => vm.ExitCommand, v => v.ExitMenuItem).DisposeWith(d);
				this.BindCommand(ViewModel, vm => vm.ExitCommand, v => v.ExitMenuItem2).DisposeWith(d);
				this.BindCommand(ViewModel, vm => vm.ExitCommand, v => v.ExitButton).DisposeWith(d);

				this.OneWayBind(ViewModel, vm => vm.ServerList, v => v.ServerListDataGrid.ItemsSource).DisposeWith(d);

				#region CloseReasonHack

				AddCloseReasonHook();

				this.Events()
					.Closing.Subscribe(e =>
					{
						if (CloseReason == CloseReason.UserClosing)
						{
							Hide();
							e.Cancel = true;
						}
					}).DisposeWith(d);

				#endregion
			});
		}

		#region CloseReasonHack

		private void AddCloseReasonHook()
		{
			if (PresentationSource.FromDependencyObject(this) is HwndSource source)
			{
				source.AddHook(WindowProc);
			}
		}

		private void RemoveCloseReasonHook()
		{
			if (PresentationSource.FromDependencyObject(this) is HwndSource source)
			{
				source.RemoveHook(WindowProc);
			}
		}

		public CloseReason CloseReason = CloseReason.None;

		private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (CloseReason != CloseReason.UserClosing && CloseReason != CloseReason.None)
			{
				RemoveCloseReasonHook();
				return IntPtr.Zero;
			}
			switch (msg)
			{
				case 0x10:
				{
					CloseReason = CloseReason.UserClosing;
					break;
				}
				case 0x11:
				case 0x16:
				{
					CloseReason = CloseReason.WindowsShutDown;
					break;
				}
				case 0x112:
				{
					if (((ushort)wParam & 0xfff0) == 0xf060)
					{
						CloseReason = CloseReason.UserClosing;
					}
					break;
				}
			}
			return IntPtr.Zero;
		}

		#endregion
	}
}
