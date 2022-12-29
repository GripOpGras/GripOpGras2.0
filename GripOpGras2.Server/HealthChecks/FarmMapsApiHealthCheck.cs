using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;
using System.Net;

namespace GripOpGras2.Server.HealthChecks
{
	internal sealed class FarmMapsApiHealthCheck : IHealthCheck
	{
		private readonly HttpClient _httpClient;

		private const int MaxResponseTimeInMilliseconds = 700;

		private const string ApiUri = "/api/v1/items";

		public FarmMapsApiHealthCheck(IHttpClientFactory httpClientFactory)
		{
			_httpClient = httpClientFactory.CreateClient("FarmMapsApi");
		}

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
			CancellationToken cancellationToken = default)
		{
			Stopwatch stopwatch = new();
			stopwatch.Start();
			try
			{
				HttpResponseMessage response = await _httpClient.GetAsync(ApiUri, cancellationToken);
				stopwatch.Stop();
				if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Unauthorized)
				{
					if (stopwatch.ElapsedMilliseconds < MaxResponseTimeInMilliseconds)
					{
						return HealthCheckResult.Healthy(
							$"The {ApiUri} endpoint responded in {stopwatch.ElapsedMilliseconds} ms.");
					}

					return HealthCheckResult.Degraded(
						$"The {ApiUri} endpoint responded in {stopwatch.ElapsedMilliseconds} ms.");
				}

				return HealthCheckResult.Unhealthy($"Failed to reach {ApiUri}. Status code: {response.StatusCode}");
			}
			catch (Exception ex)
			{
				stopwatch.Stop();
				return HealthCheckResult.Unhealthy($"Failed to reach {ApiUri} in {stopwatch.ElapsedMilliseconds}ms", ex);
			}
		}
	}
}