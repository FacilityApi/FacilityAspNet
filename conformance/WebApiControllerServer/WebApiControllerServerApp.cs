using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;
using Facility.ConformanceApi;
using Facility.ConformanceApi.Testing;
using Facility.Core;
using Microsoft.Owin.Hosting;
using Owin;

namespace WebApiControllerServer;

internal static class WebApiControllerServerApp
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
			configuration.Services.Replace(typeof(IHostBufferPolicySelector), new SseBufferPolicySelector(configuration.Services.GetHostBufferPolicySelector()));
			app.UseWebApi(configuration);
		}

		private sealed class SseBufferPolicySelector(IHostBufferPolicySelector? inner) : IHostBufferPolicySelector
		{
			public bool UseBufferedInputStream(object hostContext) => inner?.UseBufferedInputStream(hostContext) ?? false;
			public bool UseBufferedOutputStream(HttpResponseMessage response) =>
				response.Content.Headers.ContentType?.MediaType != "text/event-stream" && (inner?.UseBufferedOutputStream(response) ?? false);
		}
	}

	private static IReadOnlyList<ConformanceTestInfo> LoadTests()
	{
		using var testsJsonReader = new StreamReader(typeof(WebApiControllerServerApp).Assembly.GetManifestResourceStream("WebApiControllerServer.ConformanceTests.json")!);
		return ConformanceTestsInfo.FromJson(testsJsonReader.ReadToEnd()).Tests!;
	}
}
