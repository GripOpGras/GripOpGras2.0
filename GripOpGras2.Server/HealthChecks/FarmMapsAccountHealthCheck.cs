using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace GripOpGras2.Server.HealthChecks
{
	internal sealed class FarmMapsAccountHealthCheck : IHealthCheck
	{
		private readonly HttpClient _httpClient;

		private const int MaxResponseTimeInMilliseconds = 1200;

		private const string LoginUri = "/Account/LoginGui";

		public FarmMapsAccountHealthCheck(IHttpClientFactory httpClientFactory)
		{
			_httpClient = httpClientFactory.CreateClient("FarmMapsAccount");
		}

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
			CancellationToken cancellationToken = default)
		{
			Stopwatch stopwatch = new();
			stopwatch.Start();
			try
			{
				HttpResponseMessage response = await _httpClient.GetAsync(LoginUri, cancellationToken);
				stopwatch.Stop();
				if (response.IsSuccessStatusCode)
				{
					if (stopwatch.ElapsedMilliseconds < MaxResponseTimeInMilliseconds)
					{
						return HealthCheckResult.Healthy(
							$"The {LoginUri} endpoint responded in {stopwatch.ElapsedMilliseconds} ms.");
					}

					return HealthCheckResult.Degraded(
						$"The {LoginUri} endpoint responded in {stopwatch.ElapsedMilliseconds} ms.");
				}

				return HealthCheckResult.Unhealthy($"Failed to reach {LoginUri}. Status code: {response.StatusCode}");
			}
			catch (Exception ex)
			{
				stopwatch.Stop();
				return HealthCheckResult.Unhealthy($"Failed to reach {LoginUri} in {stopwatch.ElapsedMilliseconds}ms", ex);
			}
		}
	}
}