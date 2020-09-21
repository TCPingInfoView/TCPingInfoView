using System.Threading.Tasks;

namespace TCPingInfoView.Interfaces
{
	public interface IPluginLoader : IService
	{
		/// <summary>
		/// 加载目录中所有插件
		/// </summary>
		ValueTask LoadPluginsAsync(string dir);

		/// <summary>
		/// 加载指定路径的插件
		/// </summary>
		ValueTask LoadPluginAsync(string path);

		/// <summary>
		/// 清空已加载的插件
		/// </summary>
		ValueTask Clear();
	}
}
