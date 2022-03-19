using Mq.Geobase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mq.Geobase.Data
{
	public interface IDatabase
	{
		IReadOnlyList<IpRange> IpRanges { get; }

		IReadOnlyList<Location> Locations { get; }
	}
}
