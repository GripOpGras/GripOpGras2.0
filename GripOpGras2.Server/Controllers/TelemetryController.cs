using Microsoft.AspNetCore.Mvc;
using Prometheus;

namespace GripOpGras2.Server.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class TelemetryController : ControllerBase
	{
		private static readonly Counter RequestCountByPageRoute = Metrics
			.CreateCounter("razor_page_requests_total", "Number of requests received, by razor page route.",
				labelNames: new[] { "route" });

		[HttpPost]
		[Route("CountPageVisit")]
		public ActionResult CountPageVisit(string route)
		{
			RequestCountByPageRoute.WithLabels(route).Inc();
			return Ok();
		}
	}
}