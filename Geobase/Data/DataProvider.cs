using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Mq.Geobase.Models;

namespace Mq.Geobase.Data
{
	public class DataProvider : IDataProvider
	{
		public DataProvider(IDatabase database)
		{
			_database = database;
		}

		public Location GetLocationByIpAddress(string ipAddress)
		{
			var parsedAddress = IPAddress.Parse(ipAddress);
			var uintAddress = BitConverter.ToUInt32(parsedAddress.GetAddressBytes());

			var ipRange = FindIpRange(_database.IpRanges.ToArray(), uintAddress);
			var location = _database.GetLocationInfo(ipRange.LocationIndex);

			return location;
		}

		public IEnumerable<Location> GetCityLocations(string city)
		{
			throw new NotImplementedException();
		}

		private static IpRange FindIpRange(IpRange[] collection, uint ipAddress)
		{
			int leftBorder = 0;
			int rightBorder = collection.Length - 1;

			while (leftBorder <= rightBorder)
			{
				var middle = leftBorder + (rightBorder - leftBorder) / 2; // avoid type overflow
				var middleElement = collection[middle];
				var comparsionResult = middleElement.CompareIpAddress(ipAddress);

				if (comparsionResult == 0)
				{
					return middleElement;
				}

				if(comparsionResult == -1)
				{
					rightBorder = middle - 1;
				}
				else
				{
					leftBorder = middle + 1;
				}
			}

			return null;
		}

		private readonly IDatabase _database;
	}
}
