using ReactiveUI;
using System;
using System.Collections.Generic;
using TCPingInfoView.ViewModels;

namespace TCPingInfoView.Models
{
	[Serializable]
	public class Config : MyReactiveObject
	{
		private List<Server> _servers;

		public List<Server> Servers
		{
			get => _servers;
			set => this.RaiseAndSetIfChanged(ref _servers, value);
		}

		public Config()
		{
			Servers = new List<Server>();
		}
	}
}
