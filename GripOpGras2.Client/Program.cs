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

			builder.Services.AddOidcAuthentication(options =>
			{
				builder.Configuration.Bind("FarmMaps", options.ProviderOptions);
				options.ProviderOptions.DefaultScopes.Add("openid");
				options.ProviderOptions.DefaultScopes.Add("profile");
				options.ProviderOptions.DefaultScopes.Add("api");
			});

			await builder.Build().RunAsync();
		}
	}
}