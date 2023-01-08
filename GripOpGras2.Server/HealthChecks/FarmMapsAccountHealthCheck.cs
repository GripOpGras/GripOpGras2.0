using GripOpGras2.Server.Utils;
using Microsoft.Extensions.Diagnostics.HealthChecks;

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
			return await WebUtils.PingExternalEndpoint(LoginUri, _httpClient, MaxResponseTimeInMilliseconds,
				cancellationToken);
		}
	}
}