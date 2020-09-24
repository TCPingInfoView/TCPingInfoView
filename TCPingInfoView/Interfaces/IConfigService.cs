using System.Threading;
using System.Threading.Tasks;
using TCPingInfoView.Models;

namespace TCPingInfoView.Interfaces
{
	public interface IConfigService : IService
	{
		/// <summary>
		/// 配置文件
		/// </summary>
		Config Config { get; set; }

		/// <summary>
		/// 配置文件路径
		/// </summary>
		string FilePath { get; set; }

		/// <summary>
		/// 保存配置
		/// </summary>
		Task SaveAsync(CancellationToken token);

		/// <summary>
		/// 加载配置
		/// </summary>
		Task LoadAsync(CancellationToken token);
	}
}
