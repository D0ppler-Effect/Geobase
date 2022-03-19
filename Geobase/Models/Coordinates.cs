namespace Mq.Geobase.Models
{
	public class Coordinates
	{
		public Coordinates(float lattitude, float longitude)
		{
			Latitude = lattitude;
			Longitude = longitude;
		}

		public float Latitude { get; set; }

		public float Longitude { get; set; }
	}
}
