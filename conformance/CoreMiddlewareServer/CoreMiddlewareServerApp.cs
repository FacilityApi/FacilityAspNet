using Facility.AspNetCore;
using Facility.ConformanceApi;
using Facility.ConformanceApi.Http;
using Facility.ConformanceApi.Testing;
using Facility.Core;

namespace CoreMiddlewareServer;

public static class CoreMiddlewareServerApp
{
	public static void Main()
	{
		const string url = "http://localhost:4117";
		new WebHostBuilder().UseKestrel().UseUrls(url).UseStartup<Startup>().Build().Run();
	}

	public static readonly JsonServiceSerializer JsonSerializer = SystemTextJsonServiceSerializer.Instance;

	private sealed class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IConformanceApi>(new ConformanceApiService(new ConformanceApiServiceSettings
			{
				Tests = LoadTests(),
				JsonSerializer = JsonSerializer,
			}));
			services.AddSingleton<ConformanceApiHttpHandler>();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseFacilityExceptionHandler(includeErrorDetails: env.IsDevelopment());
			app.UseFacilityHttpHandler<ConformanceApiHttpHandler>();
		}
	}

	private static IReadOnlyList<ConformanceTestInfo> LoadTests()
	{
		using var testsJsonReader = new StreamReader(typeof(CoreMiddlewareServerApp).Assembly.GetManifestResourceStream("CoreMiddlewareServer.ConformanceTests.json")!);
		return ConformanceTestsInfo.FromJson(testsJsonReader.ReadToEnd(), JsonSerializer).Tests!;
	}
}
