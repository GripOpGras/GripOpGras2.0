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

			builder.Services.AddScoped(sp => new HttpClient
			{ BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
			builder.Services.AddTransient<IRationAlgorithm, RationAlgorithmV2>();

			builder.Services.AddOidcAuthentication(options =>
			{
				builder.Configuration.Bind("FarmMapsOidc", options.ProviderOptions);
				options.ProviderOptions.DefaultScopes.Add("openid");
				options.ProviderOptions.DefaultScopes.Add("profile");
				options.ProviderOptions.DefaultScopes.Add("api");
			});

			await builder.Build().RunAsync();
		}
	}
}