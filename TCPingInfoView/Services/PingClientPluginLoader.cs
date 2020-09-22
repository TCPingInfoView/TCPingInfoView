using PingClientBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Loader;
using System.Threading.Tasks;
using TCPingInfoView.Interfaces;
using TCPingInfoView.Utils;

namespace TCPingInfoView.Services
{
	public class PingClientPluginLoader : IPluginLoader
	{
		public readonly Dictionary<string, Func<IPingClient>> Plugins = new Dictionary<string, Func<IPingClient>>();

		public async ValueTask LoadPluginsAsync(string dir)
		{
			var files = Directory.GetFiles(dir, @"*.dll", SearchOption.AllDirectories);
			foreach (var file in files)
			{
				await LoadPluginAsync(file);
			}
		}

		public ValueTask LoadPluginAsync(string path)
		{
			try
			{
				path = Path.GetFullPath(path);
				var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
				var type = typeof(IPingClient);
				var exports = assembly.GetTypes();
				foreach (var t in exports)
				{
					if (!type.IsAssignableFrom(t)) continue;

					if (Activator.CreateInstance(t) is IPingClient client)
					{
						Plugins[client.ProtocolName] = New<IPingClient>.GetDelegate(t);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			return default;
		}

		public ValueTask Clear()
		{
			Plugins.Clear();
			return default;
		}
	}
}
