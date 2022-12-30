using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;
using System.Net;

namespace GripOpGras2.Server.Utils
{
	public static class WebUtils
	{
		public static async Task<HealthCheckResult> PingExternalEndpoint(string uri, HttpClient httpClient,
			int maxResponseTimeInMilliseconds, CancellationToken cancellationToken = default,
			params HttpStatusCode[] otherAllowedStatusCodes)
		{
			Stopwatch stopwatch = new();
			stopwatch.Start();
			try
			{
				HttpResponseMessage response = await httpClient.GetAsync(uri, cancellationToken);
				stopwatch.Stop();
				if (response.IsSuccessStatusCode || otherAllowedStatusCodes.Contains(response.StatusCode))
				{
					if (stopwatch.ElapsedMilliseconds < maxResponseTimeInMilliseconds)
					{
						return HealthCheckResult.Healthy(
							$"The {uri} endpoint responded in {stopwatch.ElapsedMilliseconds} ms.");
					}

					return HealthCheckResult.Degraded(
						$"The {uri} endpoint responded in {stopwatch.ElapsedMilliseconds} ms.");
				}

				return HealthCheckResult.Unhealthy($"Failed to reach {uri}. Status code: {response.StatusCode}");
			}
			catch (Exception ex)
			{
				stopwatch.Stop();
				return HealthCheckResult.Unhealthy($"Failed to reach {uri} in {stopwatch.ElapsedMilliseconds}ms", ex);
			}
		}
	}
}