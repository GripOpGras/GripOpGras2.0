using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;
using System.Net;

namespace GripOpGras2.Server.HealthChecks
{
	internal sealed class FarmMapsApiHealthCheck : IHealthCheck
	{
		private readonly HttpClient _httpClient;

		public FarmMapsApiHealthCheck(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
		{
			// TODO use the url from appsettings instead!!!
			const string url = "https://test.farmmaps.eu/api/v1/items/";
			Stopwatch watch = new();
			watch.Start();
			try
			{
				HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
				watch.Stop();
				if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Unauthorized)
				{
					if (watch.ElapsedMilliseconds < 500)
					{
						return HealthCheckResult.Healthy($"The {url} endpoint responded in {watch.ElapsedMilliseconds} ms.");
					}

					return HealthCheckResult.Degraded($"The {url} endpoint responded in {watch.ElapsedMilliseconds} ms.");
				}

				return HealthCheckResult.Unhealthy($"Failed to reach {url}. Status code: {response.StatusCode}");
			}
			catch (Exception ex)
			{
				watch.Stop();
				return HealthCheckResult.Unhealthy($"Failed to reach {url} in {watch.ElapsedMilliseconds}ms", ex);
			}
		}
	}
}
