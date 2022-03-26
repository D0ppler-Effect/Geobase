using Mq.Geobase.Models;
using System.Collections.Generic;

namespace Mq.Geobase.Data
{
	public interface IDatabase
	{
		IpRange[] IpRanges { get; }

		Location GetLocationOfIpRange(IpRange ipRange);

		IEnumerable<Location> FindLocationsWithSameCity(string cityName);
	}
}
