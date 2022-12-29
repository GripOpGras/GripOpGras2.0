using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace GripOpGras2.Server.HealthChecks
{
	internal sealed class FarmMapsAccountHealthCheck : IHealthCheck
	{
		private readonly HttpClient _httpClient;

		public FarmMapsAccountHealthCheck(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
		{
			// TODO use the url from appsettings instead!!!
			const string url = "https://accounts.test.farmmaps.eu/Account/LoginGui";
			Stopwatch watch = new();
			watch.Start();
			try
			{
				HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
				watch.Stop();
				if (response.IsSuccessStatusCode)
				{
					if (watch.ElapsedMilliseconds < 1000)
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
