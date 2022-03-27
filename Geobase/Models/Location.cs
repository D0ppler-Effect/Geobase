namespace Mq.Geobase.Models
{
	public class Location
	{
		public Location(
			string country,
			string region,
			string postal,
			string city,
			string organization,
			float longitude,
			float latitude)
		{
			Country = country.Trim();
			Region = region.Trim();
			Postal = postal.Trim();
			City = city.Trim();
			Organization = organization.Trim();

			Coordinates = new Coordinates(latitude, longitude);
		}

		public override string ToString()
		{
			return $"[{Country} -> {Region} -> {Postal} -> {City} -> {Organization} -> {Coordinates}]";
		}

		public string Country { get; }

		public string Region { get; }

		public string Postal { get; }

		public string City { get; }

		public string Organization { get; }

		public Coordinates Coordinates { get; }

		public const byte DbRecordSizeInBytes = 96;
	}
}