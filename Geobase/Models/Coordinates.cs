namespace Mq.Geobase.Models
{
	public class Coordinates
	{
		public Coordinates(float lattitude, float longitude)
		{
			Latitude = lattitude;
			Longitude = longitude;
		}

		public override string ToString()
		{
			return $"Lat: {Latitude}; Lon: {Longitude};";
		}

		public float Latitude { get; set; }

		public float Longitude { get; set; }
	}
}
