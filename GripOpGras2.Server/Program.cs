using GripOpGras2.Server.HealthChecks;
using Prometheus;

namespace GripOpGras2.Server
{
	public class Program
	{
		public static void Main(string[] args)
		{
			WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

			builder.Services.AddControllersWithViews();
			builder.Services.AddRazorPages();

			builder.Services.AddHttpClient("FarmMapsApi", client =>
			{
				client.BaseAddress = new Uri(builder.Configuration["FarmMapsApiBaseUrl"]);
			});
			builder.Services.AddHttpClient("FarmMapsAccount", client =>
			{
				client.BaseAddress = new Uri(builder.Configuration["FarmMapsAccountBaseUrl"]);
			});

			builder.Services.AddHealthChecks()
				.AddCheck<FarmMapsAccountHealthCheck>(nameof(FarmMapsAccountHealthCheck))
				.AddCheck<FarmMapsApiHealthCheck>(nameof(FarmMapsApiHealthCheck))
				// Report health check results in the metrics output.
				.ForwardToPrometheus();

			WebApplication app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseWebAssemblyDebugging();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();

			app.UseBlazorFrameworkFiles();
			app.UseStaticFiles();

			app.UseRouting();
			app.UseHttpMetrics();
			app.MapMetrics();

			app.MapRazorPages();
			app.MapControllers();
			app.MapFallbackToFile("index.html");

			app.Run();
		}
	}
}