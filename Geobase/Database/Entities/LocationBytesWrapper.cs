using Mq.Geobase.Models;
using System;
using System.IO;

namespace Mq.Geobase.Database.Entities
{
	public class LocationBytesWrapper
	{
		public LocationBytesWrapper(byte[] rawBytes, uint index)
		{
			Location = new Lazy<Location>(() =>
			{
				using (var stream = new MemoryStream(rawBytes))
				{
					using (var reader = new BinaryReader(stream))
					{
						var country = reader.ReadSbytes(8).ConvertToAsciiString();
						var region = reader.ReadSbytes(12).ConvertToAsciiString();
						var postal = reader.ReadSbytes(12).ConvertToAsciiString();
						var city = reader.ReadSbytes(24).ConvertToAsciiString();
						var organization = reader.ReadSbytes(32).ConvertToAsciiString();
						var latitude = reader.ReadSingle();
						var longitude = reader.ReadSingle();

						return new Location(index, country, region, postal, city, organization, longitude, latitude);
					}
				}
			});
		}

		public Lazy<Location> Location { get; }

		public const byte SizeInBytes = 96;
	}
}