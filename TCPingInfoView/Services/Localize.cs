using Microsoft.Extensions.Logging;
using System.Globalization;
using TCPingInfoView.Interfaces;

namespace TCPingInfoView.Services
{
	public class Localize : ILocalize
	{
		private readonly ILogger _logger;

		public CultureInfo Current
		{
			get => WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture;
			set
			{
				var current = WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture;
				if (!Equals(current, value))
				{
					_logger.LogDebug($@"Language {current} changed to {value}");
					WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = value;
					System.Threading.Thread.CurrentThread.CurrentCulture = value;
					System.Threading.Thread.CurrentThread.CurrentUICulture = value;
					CultureInfo.DefaultThreadCurrentCulture = value;
					CultureInfo.DefaultThreadCurrentUICulture = value;
				}
			}
		}

		public Localize(ILogger<Localize> logger)
		{
			_logger = logger;
			Current = CultureInfo.InstalledUICulture;
		}
	}
}