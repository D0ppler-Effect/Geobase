using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mq.Geobase.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mq.Geobase.Data
{
	public class BinaryLocalDatabase : IDatabase
	{
		public BinaryLocalDatabase(IConfiguration config, IWebHostEnvironment _environment, ILogger<BinaryLocalDatabase> logger)
		{
			_config = config;
			_logger = logger;

			var contentPath = _environment.ContentRootPath;
			var relativeDatabasePath = _config.GetValue<string>("DatabaseSettings:RelativeFilePath");

			ReadContents(Path.Combine(contentPath, relativeDatabasePath));
		}

		/// <summary>
		/// Uses iprange's LocationIndex field to directly read location info from binary data
		/// </summary>
		public Location GetLocationOfIpRange(IpRange ipRange)
		{
			_logger.LogInformation("Retrieving location info for ipRange '{0}' from binary database", ipRange.ToString());

			var directAddress = _locationDirectAddresses[ipRange.LocationIndex];

			return GetLocationInfoByDirectAddress(directAddress);
		}

		/// <summary>
		/// Finds first occurence of location with given city name, then searches both left+right on location index ordered by city name
		/// </summary>
		public IEnumerable<Location> FindLocationsWithSameCity(string cityName)
		{
			_logger.LogInformation("Scanning binary database for locations whithin city '{0}'", cityName);
			var resultLocations = new List<Location>();

			// find first occurence of a location with given city in locationindex array
			// for every middle'th element we read location from binary data and check city field
			_logger.LogInformation("Retrieving first occured location within city '{0}'", cityName);
			var initialLocationIndexWithGivenCity = _locationDirectAddresses.BinaryFind(
				cityName,
				(city, middleIndex) =>
				{
					var locationInfoToCheck = GetLocationInfoByIndex(middleIndex);
					return city.CompareTo(locationInfoToCheck.City);
				},
				_logger);

			if (!initialLocationIndexWithGivenCity.HasValue)
			{
				_logger.LogWarning("Locations within city '{0}' were not found in binary database!", cityName);
				return resultLocations;
			}

			_logger.LogInformation("Found first location info whithin city '{0}', scanning ordered location index for more", cityName);

			// move left over locationindex array for same city-based locations
			var leftPartTask = Task.Run(() => ScanLocations(
			   initialLocationIndexWithGivenCity.Value,
			   cityName,
			   i => i - 1));

			// move right over locationindex array for same city-based locations
			var rightPartTask = Task.Run(() => ScanLocations(
			   initialLocationIndexWithGivenCity.Value,
			   cityName,
			   i => i + 1));

			Task.WaitAll(leftPartTask, rightPartTask);

			// preserve original elemants order
			resultLocations.AddRange(leftPartTask.Result.Reverse());
			resultLocations.Add(GetLocationInfoByIndex(initialLocationIndexWithGivenCity.Value));
			resultLocations.AddRange(rightPartTask.Result);

			_logger.LogInformation("Discovered {0} locations whithin city '{1}'", resultLocations.Count, cityName);
			return resultLocations;
		}

		public IpRange[] IpRanges => _ipRanges;

		/// <summary>
		/// Read file contents into memory. Locations info is storead as raw byte array for performance reasons (we've got location direct addresses index, so we can afford it).
		/// Average database reading time (while laptop is on AC power) was between 22 and 28 milliseconds
		/// </summary>
		private void ReadContents(string filePath)
		{
			_logger.LogInformation("Starting to read database contents from file {0}", filePath);

			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException("Cannot find database file", filePath);
			}

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

				_ipRanges = new IpRange[Header.Records];
				for (var i = 0; i < Header.Records; i++)
				{
					_ipRanges[i] = ReadSingleIpRange(reader);
				}

				_locationsSection = reader.ReadBytes(Location.DbRecordSizeInBytes * Header.Records);

				_locationDirectAddresses = new uint[Header.Records];
				for (var i = 0; i < Header.Records; i++)
				{
					_locationDirectAddresses[i] = reader.ReadUInt32();
				}
			}

			var timeEnd = DateTimeOffset.UtcNow;
			var totalDurationInMs = (timeEnd - timeStart).TotalMilliseconds;

			_logger.LogInformation("Database reading completed in {0}ms", (int)totalDurationInMs);
		}

		/// <summary>
		/// Get location info by array index of its direct address
		/// </summary>
		/// <param name="locationIndex">Index of location direct address in locationDirectAddresses array</param>
		private Location GetLocationInfoByIndex(int locationIndex)
		{
			var directAddress = _locationDirectAddresses[locationIndex];

			return GetLocationInfoByDirectAddress(directAddress);
		}

		/// <summary>
		/// Read location info from binary data using its direct address
		/// </summary>
		private Location GetLocationInfoByDirectAddress(uint directAddress)
		{
			using var stream = new MemoryStream(_locationsSection);
			using (var reader = new BinaryReader(stream))
			{
				reader.BaseStream.Position = directAddress;
				return ReadSingleLocation(reader);
			}
		}

		/// <summary>
		/// Move over locations index while location contains given city and store locations to resulting collection
		/// </summary>
		/// <param name="startIndex">Initial location index to start from</param>
		/// <param name="cityName">Desired city name</param>
		/// <param name="indexMutationFunc">A function that determines search direction</param>
		/// <returns>Collection of discovered locations</returns>
		private IEnumerable<Location> ScanLocations(int startIndex, string cityName, Func<int, int> indexMutationFunc)
		{
			var resultLocations = new List<Location>();

			int currentIndex = startIndex;
			bool stopScan = false;

			while (!stopScan)
			{
				currentIndex = indexMutationFunc(currentIndex);
				
				if(currentIndex < 0 || currentIndex >= _locationDirectAddresses.Length)
				{
					break;
				}

				var location = GetLocationInfoByIndex(currentIndex);
				if (location.City.Equals(cityName, StringComparison.InvariantCulture))
				{
					resultLocations.Add(location);
				}
				else
				{
					stopScan = true;
				}
			}

			return resultLocations;
		}

		/// <summary>
		/// Read single location from binary data
		/// </summary>
		/// <param name="reader">A BinaryReader to read from</param>
		private static Location ReadSingleLocation(BinaryReader reader)
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

		/// <summary>
		/// Read single ip range info from binary data
		/// </summary>
		/// <param name="reader">A BinaryReader to read from</param>
		private static IpRange ReadSingleIpRange(BinaryReader reader)
		{
			var ipFrom = reader.ReadUInt32();
			var ipTo = reader.ReadUInt32();
			var locationIndex = reader.ReadUInt32();

			return new IpRange(ipFrom, ipTo, locationIndex);
		}

		private DatabaseHeader Header { get; set; }

		private IpRange[] _ipRanges;

		private byte[] _locationsSection;

		private uint[] _locationDirectAddresses;

		private readonly IConfiguration _config;

		private readonly ILogger<BinaryLocalDatabase> _logger;
	}
}
