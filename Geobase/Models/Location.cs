namespace Mq.Geobase.Models
{
	public class Location
	{
		// int - because database header 'records' field is typed int
		public int Index { get; set; }

		public string Country { get; set; }

		public string Region { get; set; }

		public string Postal { get; set; }

		public string City { get; set; }

		public string Organization { get; set; }

		public Coordinates Coordinates { get; set; }
	}
}