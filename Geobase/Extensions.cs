using System;
using System.Text;

namespace Mq.Geobase
{
	public static class Extensions
	{
		public static string ConvertToAsciiString(this sbyte[] sbyteArray)
		{
			var byteArray = Array.ConvertAll(sbyteArray, b => (byte)b);
			return Encoding.ASCII.GetString(byteArray).TrimEnd('\0');
		}
	}
}
