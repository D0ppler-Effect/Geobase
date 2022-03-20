namespace Mq.Geobase.Models
{
	public class Location
	{
		public Location(
			uint index,
			string country,
			string region,
			string postal,
			string city,
			string organisation,
			float longitude,
			float latitude)
		{
			Index = index;
			Country = country;
			Region = region;
			Postal = postal;
			City = city;
			Organization = organisation;

			Coordinates = new Coordinates(latitude, longitude);
		}

		public uint Index { get; }

		public string Country { get; }

		public string Region { get; }

		public string Postal { get; }

		public string City { get; }

		public string Organization { get; }

		public Coordinates Coordinates { get; }
	}
}