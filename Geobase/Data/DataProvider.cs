using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mq.Geobase.Models;

namespace Mq.Geobase.Data
{
	public class DataProvider : IDataProvider
	{
		public Location GetLocationByIpAddress(string ipAddress)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Location> GetCityLocations(string city)
		{
			return new List<Location>();
		}
	}
}
