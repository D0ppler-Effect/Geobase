using Microsoft.Extensions.Configuration;
using Mq.Geobase.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

		public IReadOnlyList<IpRange> IpRanges => _ipRanges;

		public IReadOnlyList<Location> Locations => _locations;

		private void Initialize()
		{
			var filePath = _config.GetValue<string>("DatabaseSettings:AbsoluteFilePath");

			ReadContents(filePath);
		}

		private void ReadContents(string filePath)
		{
			var timeStart = DateTimeOffset.UtcNow;
			using (var reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
			{
				_header = ReadHeader(reader);

				_ipRanges = new IpRange[_header.Records];
				for (var i = 0; i < _header.Records; i++)
				{
					_ipRanges[i] = ReadIpRange(reader);
				}

				_locations = new Location[_header.Records];
				for (var i = 0; i < _header.Records; i++)
				{
					_locations[i] = ReadLocation(reader, i);
				}
			}

			var timeEnd = DateTimeOffset.UtcNow;

			var duration = timeEnd - timeStart;
		}

		private static DatabaseHeader ReadHeader(BinaryReader reader)
		{
			var version = reader.ReadInt32();
			var name = ReadSbytes(reader, 32);
			var timestamp = reader.ReadUInt64();
			var records = reader.ReadInt32();
			var offset_ranges = reader.ReadUInt32();
			var offset_cities = reader.ReadUInt32();
			var offset_locations = reader.ReadUInt32();

			var header = new DatabaseHeader(version, name, timestamp, records, offset_ranges, offset_cities, offset_locations);

			return header;
		}

		private static IpRange ReadIpRange(BinaryReader reader)
		{
			var range = new IpRange
			{
				IpFrom = reader.ReadUInt32(),
				IpTo = reader.ReadUInt32(),
				LocationIndex = reader.ReadUInt32()
			};

			return range;
		}

		private static Location ReadLocation(BinaryReader reader, int currentIndex)
		{
			var rawData = reader.ReadBytes(Location.SizeInBytes);

			return new Location(rawData, currentIndex);
		}

		private static sbyte[] ReadSbytes(BinaryReader reader, int length)
		{
			var dataArray = new sbyte[length];
			for(var i = 0; i< length; i++)
			{
				dataArray[i] = reader.ReadSByte();
			}

			return dataArray;
		}

		private DatabaseHeader _header;

		private IpRange[] _ipRanges;

		private Location[] _locations;

		private readonly IConfiguration _config;
	}
}
