using Mq.Geobase.Models;
using System;
using System.Collections.Generic;

namespace Mq.Geobase.Data
{
	public interface IDatabase
	{
		IReadOnlyList<IpRange> IpRanges { get; }

		Location GetLocationInfo(uint locationIndex);

		IReadOnlyList<uint> LocationsIndex { get; }
	}
}
