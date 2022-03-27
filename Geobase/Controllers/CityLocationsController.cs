using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mq.Geobase.Data;
using Mq.Geobase.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mq.Geobase.Controllers
{
	[ApiController]
	[Route(route)]
	public class CityLocationsController : ControllerBase
	{
		public CityLocationsController(ILogger<CityLocationsController> logger, ILocationService dataProvider)
		{
			_logger = logger;
			_dataProvider = dataProvider;
		}

		[HttpGet]
		public ActionResult<List<Location>> Get(string city)
		{
			try
			{
				_logger.LogInformation("Received GET request for '{0}' path with '{1}' city parameter", route, city);
				
				var result = _dataProvider.GetCityLocations(city ?? string.Empty).ToList();

				_logger.LogInformation("Responding with {0} locations for request with city name '{1}'", result.Count, city);

				if (!result.Any())
				{
					return NotFound();
				}

				return SortResults(result).ToList();
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Failed to get locations for city '{0}'", city);
				return StatusCode(500, e.Message);
			}
		}

		private static IEnumerable<Location> SortResults(IEnumerable<Location> collection)
		{
			return collection
				.OrderBy(l => l.Country)
				.ThenBy(l => l.Region)
				.ThenBy(l => l.City)
				.ThenBy(l => l.Organization);
		}

		private readonly ILogger<CityLocationsController> _logger;

		private ILocationService _dataProvider;

		public const string route = "/city/locations";
	}
}
