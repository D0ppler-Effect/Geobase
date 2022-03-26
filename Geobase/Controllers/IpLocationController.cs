using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mq.Geobase.Data;
using Mq.Geobase.Models;
using System;
using System.Net;

namespace Mq.Geobase.Controllers
{
	[ApiController]
	[Route(route)]
	public class IpLocationController : ControllerBase
	{
		public IpLocationController(ILogger<IpLocationController> logger, ILocationService dataProvider)
		{
			_logger = logger;
			_dataProvider = dataProvider;
		}

		[HttpGet]
		public ActionResult<Location> Get(string ip)
		{
			try
			{
				_logger.LogInformation("Received GET request for '{0}' path with '{1}' ip address parameter", route, ip);

				var parsedAddress = IPAddress.Parse(ip);
				var result = _dataProvider.GetLocationByIpAddress(parsedAddress);

				if (result == null)
				{
					return NotFound();
				}

				_logger.LogInformation("Responding with location '{0}' for request with ip address name '{1}'", result.ToString(), ip);

				return result;
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Failed to get location for ip address '{0}'", ip);
				return StatusCode(500, e.Message);
			}
		}

		private ILocationService _dataProvider;

		private readonly ILogger<IpLocationController> _logger;

		public const string route = "/ip/location";
	}
}
