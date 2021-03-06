using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Facility.AspNetCore;
using Facility.ConformanceApi;
using Facility.ConformanceApi.Testing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace CoreControllerServer
{
	public static class CoreControllerServerApp
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
				services.AddSingleton<FacilityActionFilter>();
				services.AddMvc(options => { options.Filters.Add<FacilityActionFilter>(); }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
			}

			public void Configure(IApplicationBuilder app, IHostingEnvironment env)
			{
				app.UseFacilityExceptionHandler(includeErrorDetails: env.IsDevelopment());
				app.UseMvc();
			}
		}

		private static IReadOnlyList<ConformanceTestInfo> LoadTests()
		{
			using var testsJsonReader = new StreamReader(typeof(CoreControllerServerApp).Assembly.GetManifestResourceStream("CoreControllerServer.ConformanceTests.json")!);
			return ConformanceTestsInfo.FromJson(testsJsonReader.ReadToEnd()).Tests!;
		}
	}
}
