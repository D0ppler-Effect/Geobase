using System;

namespace Mq.Geobase.Models
{
	public class Location
	{
		public Location(byte[] rawBytes, int index)
		{
			_rawBytes = rawBytes;

			Index = index;
		}

		// int - because database header 'records' field is typed int
		public int Index { get; }

		public string Country { get; set; }

		public string Region { get; set; }

		public string Postal { get; set; }

		public string City { get; set; }

		public string Organization { get; set; }

		private byte[] _rawBytes;

		public const byte SizeInBytes = 96;
	}
}