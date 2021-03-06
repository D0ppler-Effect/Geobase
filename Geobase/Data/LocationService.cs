using Microsoft.Extensions.Logging;
using Mq.Geobase.Models;
using System;
using System.Collections.Generic;
using System.Net;

namespace Mq.Geobase.Data
{
	public class LocationService : ILocationService
	{
		public LocationService(IDatabase database, ILogger<LocationService> logger)
		{
			_database = database;
			_logger = logger;
		}
		
		/// <summary>
		/// Finds ip range that includes given ip address, then finds location of this ip range
		/// </summary>
		public Location GetLocationByIpAddress(IPAddress ipAddress)
		{
			_logger.LogInformation("Searching location info for given ip address: {0}", ipAddress);
			var uintAddress = BitConverter.ToUInt32(ipAddress.GetAddressBytes());
						
			var ipRangeIndex = _database.IpRanges.BinaryFind(
				uintAddress,
				(address, middleIndex) =>
				{
					var ipRangeToCheck = _database.IpRanges[middleIndex];
					return ipRangeToCheck.CompareIpAddress(address);
				},
				_logger);

			if (!ipRangeIndex.HasValue)
			{
				_logger.LogWarning("Ip address range for ip '{0}' was not found in database!", ipAddress);
				return null;
			}

			var ipRange = _database.IpRanges[ipRangeIndex.Value];
			var location = _database.GetLocationOfIpRange(ipRange);

			return location;
		}

		/// <summary>
		/// Finds all locations that include given city name
		/// </summary>
		public IEnumerable<Location> GetCityLocations(string cityName)
		{
			_logger.LogInformation("Searching for locations whithin city '{0}'", cityName);
			var result = _database.FindLocationsWithSameCity(cityName);

			return result;
		}

		private readonly ILogger<LocationService> _logger;

		private readonly IDatabase _database;
	}
}
