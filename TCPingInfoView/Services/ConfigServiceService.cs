using Microsoft.Extensions.Logging;
using ReactiveUI;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using TCPingInfoView.Interfaces;
using TCPingInfoView.Models;

namespace TCPingInfoView.Services
{
	[Serializable]
	public class ConfigServiceService : ReactiveObject, IConfigService, IDisposable
	{
		private Config _config;
		public Config Config
		{
			get => _config;
			set => this.RaiseAndSetIfChanged(ref _config, value);
		}

		private readonly ILogger _logger;

		private IDisposable _configMonitor;

		private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

		private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
		{
			WriteIndented = true,
			Encoder = JavaScriptEncoder.Default,
		};

		[JsonIgnore]
		public string FilePath { get; set; } = $@"{nameof(TCPingInfoView)}.config.json";

		public ConfigServiceService(ILogger<ConfigServiceService> logger)
		{
			_logger = logger;
			_configMonitor = this.WhenAnyValue(x => x.Config)
				.Throttle(TimeSpan.FromSeconds(1))
				.DistinctUntilChanged()
				.Where(_ => !_lock.IsWriteLockHeld)
				.Subscribe(async _ => { await SaveAsync(default); });
		}

		public async Task SaveAsync(CancellationToken token)
		{
			if (Config == null)
			{
				return;
			}
			try
			{
				_lock.EnterWriteLock();
				await using var stream = new MemoryStream();

				await JsonSerializer.SerializeAsync(stream, Config, JsonOptions, token);
				stream.Position = 0;

				await using var fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);

				await stream.CopyToAsync(fs, token);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, @"Save Config Error!");
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public async Task LoadAsync(CancellationToken token)
		{
			try
			{
				_lock.EnterReadLock();

				await using var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, 4096, true);

				Config = await JsonSerializer.DeserializeAsync<Config>(fs, cancellationToken: token);

				if (Config == null)
				{
					Init();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, @"Load Config Error!");
				Init();
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}

		private void Init()
		{
			Config = new Config();
		}

		public void Dispose()
		{
			_configMonitor?.Dispose();
		}
	}
}
