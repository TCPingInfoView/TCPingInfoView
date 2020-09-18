using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using ReactiveUI;
using Splat;
using TCPingInfoView.Shared;
using TCPingInfoView.Utils;

namespace TCPingInfoView
{
	public partial class App
	{
		private static int _exited;

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			Directory.SetCurrentDirectory(Path.GetDirectoryName(Utils.Utils.GetExecutablePath()));
			var identifier = $@"Global\{nameof(TCPingInfoView)}_{Directory.GetCurrentDirectory().GetDeterministicHashCode()}";

			var singleInstance = new SingleInstance.SingleInstance(identifier);
			if (!singleInstance.IsFirstInstance)
			{
				singleInstance.PassArgumentsToFirstInstance(e.Args.Append(Constants.ParameterShow));
				Current.Shutdown();
				return;
			}

			singleInstance.ArgumentsReceived.Subscribe(SingleInstance_ArgumentsReceived);
			singleInstance.ListenForArgumentsFromSuccessiveInstances();

			Current.Events().Exit.Subscribe(args => { singleInstance.Dispose(); });
			Current.Events().DispatcherUnhandledException.Subscribe(args =>
			{
				if (Interlocked.Increment(ref _exited) != 1) return;

				MessageBox.Show($@"未捕获异常：{args.Exception}", nameof(TCPingInfoView), MessageBoxButton.OK, MessageBoxImage.Error);

				singleInstance.Dispose();

				Current.Shutdown();
			});

			Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(@"##SyncfusionLicense##");

			Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());

			MainWindow = new MainWindow();
			MainWindow.ShowWindow();
		}

		private void SingleInstance_ArgumentsReceived(IEnumerable<string> args)
		{
			if (args.Contains(Constants.ParameterShow))
			{
				Dispatcher?.InvokeAsync(() => { MainWindow?.ShowWindow(); });
			}
		}
	}
}
