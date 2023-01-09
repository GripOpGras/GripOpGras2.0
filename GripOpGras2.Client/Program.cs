using GripOpGras2.Client.Features.FarmMapsLogin;
using GripOpGras2.Plugins.FarmMaps;
using GripOpGras2.Plugins.PluginInterfaces.RepositoryInterfaces;
using GripOpGras2.Client.Features.CreateRation;
using Microsoft.AspNetCore.Components.Web;
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

			builder.Services.AddScoped<FarmMapsAuthorizationMessageHandler>();

			builder.Services.AddHttpClient("FarmMapsApi",
					client =>
					{
						client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("FarmMapsBaseUrl"));
					})
				.AddHttpMessageHandler<FarmMapsAuthorizationMessageHandler>();

			// The default HttpClient is used to connect to the server-side Blazor app.
			builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

			builder.Services.AddScoped<IFarmRepository, FarmRepository>(sp =>
				new FarmRepository(sp.GetRequiredService<IHttpClientFactory>().CreateClient("FarmMapsApi")));
			
			builder.Services.AddTransient<IRationAlgorithm, RationAlgorithmV2>();
			
			builder.Services.AddOidcAuthentication(options =>
			{
				// See the following link for the supported scopes: https://accounts.test.farmmaps.eu/.well-known/openid-configuration
				builder.Configuration.Bind("FarmMapsOidc", options.ProviderOptions);
			});

			await builder.Build().RunAsync();
		}
	}
}