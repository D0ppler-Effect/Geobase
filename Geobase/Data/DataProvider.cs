using System;
using System.Collections.Generic;
using System.Linq;
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
			// find ip range given ip address belongs to and get location index

			// retrieve location by index

			return _database.GetLocationInfo(4704);
		}

		public IEnumerable<Location> GetCityLocations(string city)
		{
			var foo = _database.IpRanges.First();
			throw new NotImplementedException();
		}

		private readonly IDatabase _database;
	}
}
