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

			return _database.Locations[0].Location.Value;
		}

		public IEnumerable<Location> GetCityLocations(string city)
		{
			return _database.Locations.Take(2).Select(l => l.Location.Value);
		}

		private readonly IDatabase _database;
	}
}
