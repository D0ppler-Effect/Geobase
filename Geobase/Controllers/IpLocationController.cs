using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mq.Geobase.Models;

namespace Mq.Geobase.Controllers
{
	[ApiController]
	[Route("/ip/location")]
	public class IpLocationController : ControllerBase
	{
		private readonly ILogger<IpLocationController> _logger;

		public IpLocationController(ILogger<IpLocationController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		public Location Get(string ip)
		{
			var result = new Location {Coordinates = new Coordinates {Latitude = 1, Longitude = 1}};

			return result;
		}
	}
}
