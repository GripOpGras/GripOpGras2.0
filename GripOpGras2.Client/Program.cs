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

			//builder.Services.AddScoped(sp => new HttpClient
			//{ BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });



			//builder.Services.AddCors(options =>
			//{
			//	options.AddDefaultPolicy(
			//		builder =>
			//		{

			//			//you can configure your custom policy
			//			builder.AllowAnyOrigin()
			//				.AllowAnyHeader()
			//				.AllowAnyMethod();
			//		});
			//});

			////.............

			//app.UseCors();

			builder.Services.AddScoped<CustomAuthenticationMessageHandler>();
			builder.Services.AddHttpClient("api", client => client.BaseAddress = new Uri("https://accounts.test.farmmaps.eu/"))
				.AddHttpMessageHandler<CustomAuthenticationMessageHandler>();
			builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("api"));

			builder.Services.AddOidcAuthentication(options =>
			{
				options.ProviderOptions.Authority = "https://accounts.test.farmmaps.eu/";
				options.ProviderOptions.ClientId = "farmmapsdev";
				options.ProviderOptions.ResponseType = "code";
				options.ProviderOptions.DefaultScopes.Add("api");
				options.ProviderOptions.DefaultScopes.Add("profile");
				options.ProviderOptions.DefaultScopes.Add("email");
				options.ProviderOptions.DefaultScopes.Add("offline_access");
				options.ProviderOptions.DefaultScopes.Add("openid");

				//builder.Configuration.Bind("FarmMaps", options.ProviderOptions);
				//options.ProviderOptions.ResponseType = "code";
				////TODO mogelijk scopes toe voegen over de gebruiker profile
				//options.ProviderOptions.DefaultScopes.Add("api");
			});

			//builder.Services.AddAuthorizationCore(IISD)

			await builder.Build().RunAsync();
		}
	}

	public class CustomAuthenticationMessageHandler : AuthorizationMessageHandler
	{
		public CustomAuthenticationMessageHandler(IAccessTokenProvider provider, NavigationManager navigationManager)
			: base(provider, navigationManager)
		{
			ConfigureHandler(new[] { "https://accounts.farmmaps.eu/" });
		}
	}
}