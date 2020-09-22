using Microsoft.Extensions.Logging;
using PingClientBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using TCPingInfoView.Interfaces;
using TCPingInfoView.Utils;

namespace TCPingInfoView.Services
{
	public class PingClientPluginLoader : IPluginLoader
	{
		private readonly ILogger _logger;

		public Dictionary<string, Func<IPingClient>> Plugins { get; } = new Dictionary<string, Func<IPingClient>>();

		private static readonly Type PingClientType = typeof(IPingClient);

		public PingClientPluginLoader(ILogger<PingClientPluginLoader> logger)
		{
			_logger = logger;
		}

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
				var exports = assembly.GetTypes();

				foreach (var t in exports)
				{
					if (!IsPingClient(t))
					{
						continue;
					}

					var @delegate = New<IPingClient>.GetDelegate(t);
					var client = @delegate();

					if (!string.IsNullOrWhiteSpace(client.ProtocolName))
					{
						Plugins[client.ProtocolName] = @delegate;
						_logger.LogInformation($@"{client.ProtocolName} plugin successfully loaded");
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning($@"{ex}");
			}

			return default;
		}

		public ValueTask Clear()
		{
			Plugins.Clear();
			return default;
		}

		private static bool IsPingClient(Type type)
		{
			var typeInfo = type.GetTypeInfo();

			return
				typeInfo.IsClass &&
				!typeInfo.IsAbstract &&
				!typeInfo.IsGenericType &&
				PingClientType.IsAssignableFrom(type);
		}
	}
}
