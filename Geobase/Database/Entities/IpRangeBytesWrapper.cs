using Mq.Geobase.Models;
using System;
using System.IO;

namespace Mq.Geobase.Database.Entities
{
	public class IpRangeBytesWrapper
	{
		public IpRangeBytesWrapper(byte[] rawBytes)
		{
			IpRange = new Lazy<IpRange>(() =>
			{
				using (var stream = new MemoryStream(rawBytes))
				{
					using (var reader = new BinaryReader(stream))
					{
						var ipFrom = reader.ReadUInt32();
						var ipTo = reader.ReadUInt32();
						var locationIndex = reader.ReadUInt32();

						return new IpRange(ipFrom, ipTo, locationIndex);
					}
				}
			});
		}

		public Lazy<IpRange> IpRange { get; }

		public const byte SizeInBytes = 12;
	}
}
