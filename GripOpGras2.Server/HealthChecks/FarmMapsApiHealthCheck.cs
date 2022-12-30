using GripOpGras2.Server.Utils;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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
			return await WebUtils.PingExternalEndpoint(ApiUri, _httpClient, MaxResponseTimeInMilliseconds,
				cancellationToken, HttpStatusCode.Unauthorized);
		}
	}
}