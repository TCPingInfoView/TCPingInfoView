using Microsoft.Extensions.Logging;
using System.Globalization;
using TCPingInfoView.Interfaces;

namespace TCPingInfoView.Services
{
	public class Localize : ILocalize
	{
		private readonly ILogger _logger;

		private CultureInfo _current;

		public CultureInfo Current
		{
			get => _current;
			set
			{
				if (!Equals(_current, value))
				{
					_logger.LogDebug($@"Language {_current} changed to {value}");
					_current = value;
					WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = value;
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