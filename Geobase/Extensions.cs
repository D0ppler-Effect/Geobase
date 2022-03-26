using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
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

		public static IPAddress ToIpAddress(this uint value)
		{
			var ipBytes = BitConverter.GetBytes(value);
			var result = new IPAddress(ipBytes);

			return result;
		}

		public static int? BinaryFind<TArrayType, TDesired>(
			this TArrayType[] arrayToSearchWithin,
			TDesired valueToSearchFor,
			Func<TDesired, int, int> valueToElementByIndexCompareAction,
			ILogger logger = null)
		{
			int leftBorder = 0;
			int rightBorder = arrayToSearchWithin.Length - 1;

			while (leftBorder <= rightBorder)
			{
				var middle = leftBorder + (rightBorder - leftBorder) / 2; // avoid type overflow

				var comparsionResult = valueToElementByIndexCompareAction(valueToSearchFor, middle);

				if (comparsionResult == 0)
				{
					return middle;
				}

				if (comparsionResult == -1)
				{
					rightBorder = middle - 1;
				}
				else
				{
					leftBorder = middle + 1;
				}
			}

			logger?.LogWarning($"Error: desired value '{valueToSearchFor}' wasn't found in collection of type {typeof(TArrayType).FullName}");
			
			return null;
		}
	}
}
