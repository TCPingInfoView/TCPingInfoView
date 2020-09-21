using System;
using System.Linq.Expressions;
using PingClientBase;

namespace TCPingInfoView.Utils
{
	public static class NewIPingClient
	{
		public static Func<IPingClient> GetDelegate(Type type) => Expression.Lambda<Func<IPingClient>>(Expression.New(type)).Compile();
	}
}
