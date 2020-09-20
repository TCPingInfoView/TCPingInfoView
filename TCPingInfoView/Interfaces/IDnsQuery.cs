using System.Net;
using System.Threading.Tasks;

namespace TCPingInfoView.Interfaces
{
	public interface IDnsQuery : IService
	{
		public ValueTask<IPAddress> QueryAsync(string host);
	}
}
