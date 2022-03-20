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

		public uint IpFrom { get; set; }

		public uint IpTo { get; set; }

		public uint LocationIndex { get; set; }
	}
}
