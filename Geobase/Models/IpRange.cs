using System;

namespace Mq.Geobase.Models
{
	public class IpRange
	{
		public IpRange(uint ipFrom, uint ipTo, uint locationIndex)
		{
			IpFrom = ipFrom;
			IpTo = ipTo;
			LocationIndex = locationIndex;
		}

		public bool ContainsIp(uint ipAddress)
		{
			return ipAddress >= IpFrom && ipAddress <= IpTo;
		}

		public int CompareIpAddress(uint value)
		{
			if(value < IpFrom)
			{
				return -1;
			}
			else if (value > IpTo)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		public uint IpFrom { get; set; }

		public uint IpTo { get; set; }

		public uint LocationIndex { get; set; }

		public const byte DbRecordSizeInBytes = 12;
	}
}
