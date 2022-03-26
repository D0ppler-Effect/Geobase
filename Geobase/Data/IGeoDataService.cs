using Mq.Geobase.Models;
using System.Collections.Generic;
using System.Net;

namespace Mq.Geobase.Data
{
	public interface IGeoDataService
	{
		public Location GetLocationByIpAddress(IPAddress ipAddress);

		public IEnumerable<Location> GetCityLocations(string city);
	}
}
