using System.Globalization;

namespace TCPingInfoView.Interfaces
{
	public interface ILocalize : IService
	{
		CultureInfo Current { get; set; }
	}
}
