using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mq.Geobase.Models;
using Mq.Geobase.Data;

namespace Mq.Geobase.Controllers
{
	[ApiController]
	[Route("/ip/location")]
	public class IpLocationController : ControllerBase
	{
		private readonly ILogger<IpLocationController> _logger;

		public IpLocationController(ILogger<IpLocationController> logger, IDataProvider dataProvider)
		{
			_logger = logger;
			_dataProvider = dataProvider;
		}

		[HttpGet]
		public Location Get(string ip)
		{
			var result = _dataProvider.GetLocationByIpAddress(ip);
			return result;
		}

		private IDataProvider _dataProvider;
	}
}
