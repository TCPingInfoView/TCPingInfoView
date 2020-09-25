using Syncfusion.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Numerics;
using TCPingInfoView.Models;

namespace TCPingInfoView.Utils
{
	public class CustomComparer : IComparer<object>, ISortDirection
	{
		public int Compare(object x, object y)
		{
			while (true)
			{
				int comparer;
				if (x is IPAddress ipX && y is IPAddress ipY)
				{
					comparer = ToBigInteger(ipX).CompareTo(ToBigInteger(ipY));
				}
				else if (x is Server sX && y is Server sY)
				{
					x = sX.Ip;
					y = sY.Ip;
					continue;
				}
				else if (x == null && y == null)
				{
					comparer = 0;
				}
				else if (x == null)
				{
					comparer = -1;
				}
				else if (y == null)
				{
					comparer = 1;
				}
				else
				{
					comparer = string.CompareOrdinal(x.ToString(), y.ToString());
				}

				//returns the comparison result based in SortDirection.

				if (comparer > 0)
				{
					return SortDirection == ListSortDirection.Ascending ? 1 : -1;
				}

				if (comparer < 0)
				{
					return SortDirection == ListSortDirection.Ascending ? -1 : 1;
				}

				return 0;
			}
		}

		public ListSortDirection SortDirection { get; set; }

		private static BigInteger ToBigInteger(IPAddress ip)
		{
			return ip == null ? BigInteger.Zero : new BigInteger(ip.GetAddressBytes().AsSpan(), true, true);
		}
	}
}
