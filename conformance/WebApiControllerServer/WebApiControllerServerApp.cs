using System.Web.Http;
using Facility.ConformanceApi;
using Facility.ConformanceApi.Testing;
using Facility.Core;
using Microsoft.Owin.Hosting;
using Owin;

namespace WebApiControllerServer;

public static class WebApiControllerServerApp
{
	public static void Main()
	{
		const string url = "http://localhost:4117";
		using (WebApp.Start<Startup>(url))
		{
			Console.WriteLine($"Server started: {url}");
			Console.WriteLine("Press a key to stop.");
			Console.ReadKey();
		}
	}

	public static readonly JsonServiceSerializer JsonSerializer = SystemTextJsonServiceSerializer.Instance;

	public static readonly IConformanceApi Service = new ConformanceApiService(
		new ConformanceApiServiceSettings
		{
			Tests = LoadTests(),
			JsonSerializer = JsonSerializer,
		});

	private sealed class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			var configuration = new HttpConfiguration();
			configuration.MapHttpAttributeRoutes();
			app.UseWebApi(configuration);
		}
	}

	private static IReadOnlyList<ConformanceTestInfo> LoadTests()
	{
		using (var testsJsonReader = new StreamReader(typeof(WebApiControllerServerApp).Assembly.GetManifestResourceStream("WebApiControllerServer.ConformanceTests.json")))
			return ConformanceTestsInfo.FromJson(testsJsonReader.ReadToEnd(), JsonSerializer).Tests!;
	}
}
