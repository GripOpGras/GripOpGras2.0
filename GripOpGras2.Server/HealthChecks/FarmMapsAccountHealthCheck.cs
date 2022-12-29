using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace GripOpGras2.Server.HealthChecks
{
	internal sealed class FarmMapsAccountHealthCheck : IHealthCheck
	{
		private readonly HttpClient _httpClient;

		private const int MaxResponseTimeInMilliseconds = 1200;

		public FarmMapsAccountHealthCheck(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
			CancellationToken cancellationToken = default)
		{
			// TODO use the url from appsettings instead!!!
			const string url = "https://accounts.test.farmmaps.eu/Account/LoginGui";
			Stopwatch stopwatch = new();
			stopwatch.Start();
			try
			{
				HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
				stopwatch.Stop();
				if (response.IsSuccessStatusCode)
				{
					if (stopwatch.ElapsedMilliseconds < MaxResponseTimeInMilliseconds)
					{
						return HealthCheckResult.Healthy(
							$"The {url} endpoint responded in {stopwatch.ElapsedMilliseconds} ms.");
					}

					return HealthCheckResult.Degraded(
						$"The {url} endpoint responded in {stopwatch.ElapsedMilliseconds} ms.");
				}

				return HealthCheckResult.Unhealthy($"Failed to reach {url}. Status code: {response.StatusCode}");
			}
			catch (Exception ex)
			{
				stopwatch.Stop();
				return HealthCheckResult.Unhealthy($"Failed to reach {url} in {stopwatch.ElapsedMilliseconds}ms", ex);
			}
		}
	}
}