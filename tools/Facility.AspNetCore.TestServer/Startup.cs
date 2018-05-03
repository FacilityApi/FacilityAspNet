using Facility.Core.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TestServerApi;
using TestServerApi.Http;

namespace Facility.AspNetCore.TestServer
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<ITestServerApi, TestServerApi>();
			services.AddSingleton<ServiceHttpHandlerSettings>();
			services.AddSingleton<TestServerApiHttpHandler>();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();

			app.UseFacilityHttpHandler<TestServerApiHttpHandler>();
		}
	}
}
