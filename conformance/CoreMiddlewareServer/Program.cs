using Facility.AspNetCore;
using Facility.ConformanceApi;
using Facility.ConformanceApi.Http;
using Facility.ConformanceApi.Testing;
using Facility.Core;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddSingleton<IConformanceApi>(_ =>
{
	using var testsJsonReader = new StreamReader(typeof(Program).Assembly.GetManifestResourceStream("CoreMiddlewareServer.ConformanceTests.json")!);
	return new ConformanceApiService(new ConformanceApiServiceSettings
	{
		Tests = ConformanceTestsInfo.FromJson(testsJsonReader.ReadToEnd()).Tests!,
		JsonSerializer = SystemTextJsonServiceSerializer.Instance,
	});
});

services.AddSingleton<ConformanceApiHttpHandler>();

var app = builder.Build();

app.UseFacilityExceptionHandler(includeErrorDetails: !app.Environment.IsProduction());

app.UseFacilityHttpHandler<ConformanceApiHttpHandler>();

app.Run();
