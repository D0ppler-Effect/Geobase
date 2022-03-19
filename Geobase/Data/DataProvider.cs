using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
			return _database.Locations[0];
		}

		public IEnumerable<Location> GetCityLocations(string city)
		{
			return _database.Locations.Take(2);
		}

		private readonly IDatabase _database;
	}
}
