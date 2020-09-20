using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TCPingInfoView.Interfaces;

namespace TCPingInfoView.Services
{
	public class DefaultDnsQuery : IDnsQuery
	{
		public async ValueTask<IPAddress> QueryAsync(string host)
		{
			try
			{
				var ip = IsIPAddress(host);
				if (ip != null)
				{
					return ip;
				}

				var res = await Dns.GetHostAddressesAsync(host);
				return res.FirstOrDefault();
			}
			catch
			{
				return null;
			}
		}

		private static IPAddress IsIPAddress(string host)
		{
			if (host != null && IPAddress.TryParse(host, out var ip))
			{
				return ip;
			}

			return null;
		}
	}
}
