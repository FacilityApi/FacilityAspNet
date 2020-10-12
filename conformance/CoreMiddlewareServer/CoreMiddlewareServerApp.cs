using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Facility.AspNetCore;
using Facility.ConformanceApi;
using Facility.ConformanceApi.Http;
using Facility.ConformanceApi.Testing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CoreMiddlewareServer
{
	public static class CoreMiddlewareServerApp
	{
		public static void Main()
		{
			const string url = "http://localhost:4117";
			new WebHostBuilder().UseKestrel().UseUrls(url).UseStartup<Startup>().Build().Run();
		}

		private sealed class Startup
		{
			public void ConfigureServices(IServiceCollection services)
			{
				services.AddSingleton<IConformanceApi>(new ConformanceApiService(LoadTests()));
				services.AddSingleton<ConformanceApiHttpHandler>();
			}

			[SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Hosting environment not currently used.")]
			public void Configure(IApplicationBuilder app, IHostingEnvironment env)
			{
				app.UseFacilityHttpHandler<ConformanceApiHttpHandler>();
			}
		}

		private static IReadOnlyList<ConformanceTestInfo> LoadTests()
		{
			using var testsJsonReader = new StreamReader(typeof(CoreMiddlewareServerApp).Assembly.GetManifestResourceStream("CoreMiddlewareServer.ConformanceTests.json")!);
			return ConformanceTestsInfo.FromJson(testsJsonReader.ReadToEnd()).Tests!;
		}
	}
}
