using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace GripOpGras2.Client
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("#app");
			builder.RootComponents.Add<HeadOutlet>("head::after");

			builder.Services.AddScoped<CustomAuthorizationMessageHandler>();

			builder.Services.AddHttpClient("FarmMapsApi", client =>
				{
					client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("FarmMapsBaseUrl"));
				})
				.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

			builder.Services.AddScoped(
				sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("FarmMapsApi"));

			builder.Services.AddOidcAuthentication(options =>
			{
				// See the following link for the supported scopes: https://accounts.test.farmmaps.eu/.well-known/openid-configuration
				builder.Configuration.Bind("FarmMapsOidc", options.ProviderOptions);
			});

			await builder.Build().RunAsync();
		}
	}

	public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
	{
		public CustomAuthorizationMessageHandler(IAccessTokenProvider provider,
			NavigationManager navigationManager)
			: base(provider, navigationManager)
		{
			ConfigureHandler(
				authorizedUrls: new[] { "https://accounts.test.farmmaps.eu", "https://test.farmmaps.eu" }
			);

		}
	}
}