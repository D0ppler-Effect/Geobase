using Mq.Geobase.Models;
using System;
using System.Collections.Generic;

namespace Mq.Geobase.Data
{
	public interface IDatabase
	{
		IpRange[] IpRanges { get; }

		Location GetLocationInfo(uint locationIndex);

		uint[] LocationsIndex { get; }
	}
}
