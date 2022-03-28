using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Mq.Geobase
{
	public static class Extensions
	{
		/// <summary>
		/// converts array of sbytes to ASCII string
		/// </summary>
		public static string ConvertToAsciiString(this sbyte[] sbyteArray)
		{
			var byteArray = Array.ConvertAll(sbyteArray, b => (byte)b);
			return Encoding.ASCII.GetString(byteArray).TrimEnd('\0');
		}

		/// <summary>
		/// Read an sbyte section of given length
		/// </summary>
		/// <param name="reader">a BinaryReader to read from</param>
		/// <param name="length">desired length</param>
		public static sbyte[] ReadSbytes(this BinaryReader reader, int length)
		{
			var dataArray = new sbyte[length];
			for (var i = 0; i < length; i++)
			{
				dataArray[i] = reader.ReadSByte();
			}

			return dataArray;
		}

		/// <summary>
		/// Parse uint into system.IO IPAddress type
		/// </summary>
		public static IPAddress ToIpAddress(this uint value)
		{
			var ipBytes = BitConverter.GetBytes(value);
			var result = new IPAddress(ipBytes);

			return result;
		}

		/// <summary>
		/// Performs a binary search on given array
		/// </summary>
		/// <typeparam name="TArrayType">Type of elements in given array</typeparam>
		/// <typeparam name="TDesired">Type of value to look for</typeparam>
		/// <param name="arrayToSearchWithin">Array to be searched</param>
		/// <param name="valueToSearchFor">Argument being passed to a function used for comparsion</param>
		/// <param name="valueToElementByIndexCompareAction">Comparsion criteria, called on middle element of array for every search iteration,
		/// returns -1, 0, 1 to determine further search direction</param>
		/// <param name="logger">Optional logger</param>
		/// <returns>Index of array element which, being passed to a valueToElementByIndexCompareAction, returns 0. Returns null if nothing found.</returns>
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
				var middleIndex = leftBorder + (rightBorder - leftBorder) / 2; // avoid type overflow

				var comparsionResult = valueToElementByIndexCompareAction(valueToSearchFor, middleIndex);

				if (comparsionResult == 0)
				{
					return middleIndex;
				}

				if (comparsionResult == -1)
				{
					rightBorder = middleIndex - 1;
				}
				else
				{
					leftBorder = middleIndex + 1;
				}
			}

			logger?.LogWarning($"Error: desired value '{valueToSearchFor}' wasn't found in collection of type {typeof(TArrayType).FullName}");
			
			return null;
		}
	}
}
