using System;
using System.IO;
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

		public static sbyte[] ReadSbytes(this BinaryReader reader, int length)
		{
			var dataArray = new sbyte[length];
			for (var i = 0; i < length; i++)
			{
				dataArray[i] = reader.ReadSByte();
			}

			return dataArray;
		}
	}
}
