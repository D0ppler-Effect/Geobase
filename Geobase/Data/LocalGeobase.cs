using Microsoft.Extensions.Configuration;
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

			ParsedIpRanges = new Lazy<IpRange[]>(ParseIpRangesSection, true);
		}

		public Location GetLocationInfo(uint locationIndex)
		{
			var directAddress = _locationsIndex[(int)locationIndex];

			using var stream = new MemoryStream(_locationsSection);
			using (var reader = new BinaryReader(stream))
			{
				reader.BaseStream.Position = directAddress;
				return ReadSingleLocation(reader);
			}
		}

		public IpRange[] IpRanges => ParsedIpRanges.Value;

		public uint[] LocationsIndex => _locationsIndex;

		private void Initialize()
		{
			ReadContents(_config.GetValue<string>("DatabaseSettings:AbsoluteFilePath"));
		}

		private void ReadContents(string filePath)
		{
			DateTimeOffset timeStart = DateTimeOffset.UtcNow;
			using (var reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
			{
				var version = reader.ReadInt32();
				var name = reader.ReadSbytes(32);
				var timestamp = reader.ReadUInt64();
				var records = reader.ReadInt32();
				var offset_ranges = reader.ReadUInt32();
				var offset_cities = reader.ReadUInt32();
				var offset_locations = reader.ReadUInt32();

				Header = new DatabaseHeader(version, name, timestamp, records, offset_ranges, offset_cities, offset_locations);

				_ipRangesSection = reader.ReadBytes(IpRange.DbRecordSizeInBytes * Header.Records);

				_locationsSection = reader.ReadBytes(Location.DbRecordSizeInBytes * Header.Records);

				_locationsIndex = new uint[Header.Records];
				for (var i = 0; i < Header.Records; i++)
				{
					_locationsIndex[i] = reader.ReadUInt32();
				}
			}

			var timeEnd = DateTimeOffset.UtcNow;
			var totalDuration = timeEnd - timeStart;
		}

		private IpRange[] ParseIpRangesSection()
		{
			using var stream = new MemoryStream(_ipRangesSection);
			using (var reader = new BinaryReader(stream))
			{
				var result = new IpRange[Header.Records];
				for (var i = 0; i < Header.Records; i++)
				{
					result[i] = ReadSingleIpRange(reader);
				}

				return result;
			}
		}

		private Location ReadSingleLocation(BinaryReader reader)
		{
			var country = reader.ReadSbytes(8).ConvertToAsciiString();
			var region = reader.ReadSbytes(12).ConvertToAsciiString();
			var postal = reader.ReadSbytes(12).ConvertToAsciiString();
			var city = reader.ReadSbytes(24).ConvertToAsciiString();
			var organization = reader.ReadSbytes(32).ConvertToAsciiString();
			var latitude = reader.ReadSingle();
			var longitude = reader.ReadSingle();

			return new Location(country, region, postal, city, organization, longitude, latitude);
		}

		private IpRange ReadSingleIpRange(BinaryReader reader)
		{
			var ipFrom = reader.ReadUInt32();
			var ipTo = reader.ReadUInt32();
			var locationIndex = reader.ReadUInt32();

			return new IpRange(ipFrom, ipTo, locationIndex);
		}

		private DatabaseHeader Header { get; set; }

		private Lazy<IpRange[]> ParsedIpRanges { get; }

		private byte[] _ipRangesSection;

		private byte[] _locationsSection;

		private uint[] _locationsIndex;

		private readonly IConfiguration _config;
	}
}
