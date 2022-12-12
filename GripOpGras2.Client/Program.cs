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
			builder.Services.AddTransient<IRationAlgorithm, RationAlgorithmV1>();

			await builder.Build().RunAsync();
		}
	}
}