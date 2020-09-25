using PingClientBase;
using ReactiveUI;
using System;
using System.Net;
using System.Text.Json.Serialization;
using TCPingInfoView.ViewModels;

namespace TCPingInfoView.Models
{
	[Serializable]
	public class Server : MyReactiveObject
	{
		#region 字段

		private string _hostname;
		private ushort _port;
		private string _protocol;
		private string _remark;
		private IPAddress _ip;
		private PingResult _currentResult;
		private int _index;

		#endregion

		#region 属性

		/// <summary>
		/// 主机名，域名、IP
		/// </summary>
		public string Hostname
		{
			get => _hostname;
			set => this.RaiseAndSetIfChanged(ref _hostname, value);
		}

		/// <summary>
		/// 端口
		/// </summary>
		public ushort Port
		{
			get => _port;
			set => this.RaiseAndSetIfChanged(ref _port, value);
		}

		/// <summary>
		/// 协议名
		/// </summary>
		public string Protocol
		{
			get => _protocol;
			set => this.RaiseAndSetIfChanged(ref _protocol, value);
		}

		/// <summary>
		/// 备注
		/// </summary>
		public string Remark
		{
			get => _remark;
			set => this.RaiseAndSetIfChanged(ref _remark, value);
		}

		/// <summary>
		/// 响应 IP
		/// </summary>
		[JsonIgnore]
		public IPAddress Ip
		{
			get => _ip;
			set => this.RaiseAndSetIfChanged(ref _ip, value);
		}

		/// <summary>
		/// 当前返回的结果
		/// </summary>
		[JsonIgnore]
		public PingResult CurrentResult
		{
			get => _currentResult;
			set => this.RaiseAndSetIfChanged(ref _currentResult, value);
		}

		/// <summary>
		/// 在列表的位置
		/// </summary>
		[JsonIgnore]
		public int Index
		{
			get => _index;
			set => this.RaiseAndSetIfChanged(ref _index, value);
		}

		#endregion
	}
}
