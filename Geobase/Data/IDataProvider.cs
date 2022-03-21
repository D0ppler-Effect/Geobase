using System.Collections.Generic;
using Mq.Geobase.Models;

namespace Mq.Geobase.Data
{
	public interface IDataProvider
	{
		public Location GetLocationByIpAddress(string ipAddress);

		public IEnumerable<Location> GetCityLocations(string city);
	}
}
