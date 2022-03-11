using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mq.Geobase.Data;
using Mq.Geobase.Models;

namespace Mq.Geobase.Controllers
{
	[ApiController]
	[Route("/city/locations")]
	public class CityLocationsController : ControllerBase
	{
		private readonly ILogger<CityLocationsController> _logger;

		public CityLocationsController(ILogger<CityLocationsController> logger, IDataProvider dataProvider)
		{
			_logger = logger;
			_dataProvider = dataProvider;
		}

		[HttpGet]
		public IEnumerable<Location> Get(string city)
		{
			var result = _dataProvider.GetCityLocations(city);
			return new[] {new Location {Coordinates = new Coordinates {Latitude = 2, Longitude = 2}}};
		}

		private IDataProvider _dataProvider;
	}
}
