using System;
using System.Linq.Expressions;

namespace TCPingInfoView.Utils
{
	public static class New<T>
	{
		public static Func<T> GetDelegate(Type type) => Expression.Lambda<Func<T>>(Expression.New(type)).Compile();
	}
}
