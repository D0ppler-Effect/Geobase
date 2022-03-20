using Microsoft.Extensions.Configuration;
using Mq.Geobase.Database.Entities;
using Mq.Geobase.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace Mq.Geobase.Data
{
	public class LocalGeobase : IDatabase
	{
		public LocalGeobase(IConfiguration config)
		{
			_config = config;

			Initialize();
		}

		public IReadOnlyList<IpRangeBytesWrapper> IpRanges => _ipRanges;

		public IReadOnlyList<LocationBytesWrapper> Locations => _locations;

		public IReadOnlyList<int> LocationsIndex => _locationsIndex;

		private void Initialize()
		{
			var filePath = _config.GetValue<string>("DatabaseSettings:AbsoluteFilePath");

			ReadContents(filePath);
		}

		private void ReadContents(string filePath)
		{
			DateTimeOffset timeStart = DateTimeOffset.UtcNow;
			DateTimeOffset timeDbReady;
			DateTimeOffset headerReady;
			DateTimeOffset ipRangesReady;
			using (var reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
			{
				timeDbReady = DateTimeOffset.UtcNow;

				_header = ReadHeader(reader);

				headerReady = DateTimeOffset.UtcNow;

				_ipRanges = new IpRangeBytesWrapper[_header.Records];
				for (var i = 0; i < _header.Records; i++)
				{
					var rawData = reader.ReadBytes(IpRangeBytesWrapper.SizeInBytes);
					_ipRanges[i] = new IpRangeBytesWrapper(rawData);
				}

				ipRangesReady = DateTimeOffset.UtcNow;

				_locations = new LocationBytesWrapper[_header.Records];
				for (var i = 0; i < _header.Records; i++)
				{
					var index = (uint)(reader.BaseStream.Position - _header.Offset_locations);

					var rawData = reader.ReadBytes(LocationBytesWrapper.SizeInBytes);
					_locations[i] = new LocationBytesWrapper(rawData, index);
				}

				_locationsIndex = new int[_header.Records];
				for (var i = 0; i < _header.Records; i++)
				{
					_locationsIndex[i] = reader.ReadInt32();
				}
			}

			var timeEnd = DateTimeOffset.UtcNow;

			var fileReadDuration = timeDbReady - timeStart;
			var headerReadDuration = headerReady - timeDbReady;
			var ipRangesReadDuration = ipRangesReady - headerReady;
			var locationsReadDuration = timeEnd - ipRangesReady;
			var totalDuration = timeEnd - timeStart;
		}

		private static DatabaseHeader ReadHeader(BinaryReader reader)
		{
			var version = reader.ReadInt32();
			var name = reader.ReadSbytes(32);
			var timestamp = reader.ReadUInt64();
			var records = reader.ReadInt32();
			var offset_ranges = reader.ReadUInt32();
			var offset_cities = reader.ReadUInt32();
			var offset_locations = reader.ReadUInt32();

			var header = new DatabaseHeader(version, name, timestamp, records, offset_ranges, offset_cities, offset_locations);

			return header;
		}
				
		private DatabaseHeader _header;

		private IpRangeBytesWrapper[] _ipRanges;

		private LocationBytesWrapper[] _locations;

		private int[] _locationsIndex;

		private readonly IConfiguration _config;
	}
}
