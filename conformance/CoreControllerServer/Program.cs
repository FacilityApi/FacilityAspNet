using Facility.AspNetCore;
using Facility.ConformanceApi;
using Facility.ConformanceApi.Testing;
using Facility.Core;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddSingleton<IConformanceApi>(_ =>
{
	using var testsJsonReader = new StreamReader(typeof(Program).Assembly.GetManifestResourceStream("CoreControllerServer.ConformanceTests.json")!);
	return new ConformanceApiService(new ConformanceApiServiceSettings
	{
		Tests = ConformanceTestsInfo.FromJson(testsJsonReader.ReadToEnd(), SystemTextJsonServiceSerializer.Instance).Tests!,
		JsonSerializer = SystemTextJsonServiceSerializer.Instance,
	});
});

services.AddControllers(options => options.Filters.Add<FacilityActionFilter>())
	.AddJsonOptions(options => SystemTextJsonServiceSerializer.ConfigureJsonSerializerOptions(options.JsonSerializerOptions));

services.ConfigureHttpJsonOptions(options => SystemTextJsonServiceSerializer.ConfigureJsonSerializerOptions(options.SerializerOptions));

var app = builder.Build();

app.UseFacilityExceptionHandler(includeErrorDetails: !app.Environment.IsProduction());

app.UseRouting();

app.MapControllers();

// should return 200 with {"id":1} (no name)
app.MapGet("minimal/dto", () => new WidgetDto { Id = 1 });

// should return 200 with {"id":1} (no name)
app.MapGet("minimal/error", () => ServiceErrors.CreateRequestTooLarge())
	.AddEndpointFilter<FacilityEndpointFilter>();

// should return 413 with {"code":"RequestTooLarge","message":"The request is too large."}
app.MapGet("minimal/success", () => ServiceResult.Success(new WidgetDto { Id = 1 }))
	.AddEndpointFilter<FacilityEndpointFilter>();

// should return 413 with {"code":"RequestTooLarge","message":"The request is too large."}
app.MapGet("minimal/failure", () => ServiceResult.Failure(ServiceErrors.CreateRequestTooLarge()))
	.AddEndpointFilter<FacilityEndpointFilter>();

app.Run();
