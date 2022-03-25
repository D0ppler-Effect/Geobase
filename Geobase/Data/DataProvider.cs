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

			var ipRangeIndex = BinaryFind(
				_database.IpRanges,
				uintAddress,
				(address, i) =>
					{
						var middleElement = _database.IpRanges[i];
						return middleElement.CompareIpAddress(address);
					});

			var location = _database.GetLocationInfo(_database.IpRanges[ipRangeIndex].LocationIndex);

			return location;
		}

		public IEnumerable<Location> GetCityLocations(string cityName)
		{
			var initialLocationIndex = BinaryFind(
				_database.LocationsIndex,
				cityName,
				(city, i) =>
					{
						var locationInfo = _database.GetLocationInfo(i);
						return city.CompareTo(locationInfo.City);
					});

			return null;
		}

		private static int BinaryFind<TArrayType, TDesired>(
			TArrayType[] arrayToSearch, 
			TDesired desiredObject,
			Func<TDesired, int, int> elementByIndexComparer)
		{
			int leftBorder = 0;
			int rightBorder = arrayToSearch.Length - 1;

			while (leftBorder <= rightBorder)
			{
				var middle = leftBorder + (rightBorder - leftBorder) / 2; // avoid type overflow

				var comparsionResult = elementByIndexComparer(desiredObject, middle);

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

			throw new Exception("Database search error");
		}
				
		private readonly IDatabase _database;
	}
}
