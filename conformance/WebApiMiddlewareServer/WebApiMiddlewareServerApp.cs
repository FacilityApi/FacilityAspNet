using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;
using Facility.ConformanceApi.Http;
using Facility.ConformanceApi.Testing;
using Facility.Core;
using Microsoft.Owin.Hosting;
using Owin;

namespace WebApiMiddlewareServer;

public static class WebApiMiddlewareServerApp
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

	private sealed class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			var configuration = new HttpConfiguration();
			configuration.MessageHandlers.Add(new ConformanceApiHttpHandler(new ConformanceApiService(new ConformanceApiServiceSettings
			{
				Tests = LoadTests(),
				JsonSerializer = JsonSerializer,
			})));
			configuration.Services.Replace(typeof(IHostBufferPolicySelector), new NoBufferPolicySelector());
			app.UseWebApi(configuration);
		}

		private sealed class NoBufferPolicySelector : IHostBufferPolicySelector
		{
			public bool UseBufferedInputStream(object hostContext) => false;
			public bool UseBufferedOutputStream(HttpResponseMessage response) => false; //// required for event streaming
		}
	}

	private static IReadOnlyList<ConformanceTestInfo> LoadTests()
	{
		using var testsJsonReader = new StreamReader(typeof(WebApiMiddlewareServerApp).Assembly.GetManifestResourceStream("WebApiMiddlewareServer.ConformanceTests.json")!);
		return ConformanceTestsInfo.FromJson(testsJsonReader.ReadToEnd()).Tests!;
	}
}
