using System;

namespace Mq.Geobase.Models
{
	public class DatabaseHeader
	{
		public DatabaseHeader(
			int version,
			sbyte[] name,
			ulong timestamp,
			int records,
			uint offset_ranges,
			uint offset_cities,
			uint offset_locations)
		{
			Version = version;
			Name = name.ConvertToAsciiString();
			Timestamp = timestamp;
			Records = records;
			Offset_cities = offset_cities;
			Offset_locations = offset_locations;
			Offset_ranges = offset_ranges;
		}

		public int Version { get; }

		public string Name { get; }

		public ulong Timestamp { get; } 

		public int Records { get; } 

		public uint Offset_ranges { get; } 

		public uint Offset_cities { get; }

		public uint Offset_locations { get; } 

		public const int HeaderSizeInBytes = 60;
	}
}
