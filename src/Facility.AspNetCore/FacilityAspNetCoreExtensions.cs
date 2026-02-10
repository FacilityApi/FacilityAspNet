using Facility.Core;
using Facility.Core.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Facility.AspNetCore;

/// <summary>
/// ASP.NET Core extension methods for Facility.
/// </summary>
public static class FacilityAspNetCoreExtensions
{
	/// <summary>
	/// Adds a Facility service HTTP handler to the pipeline.
	/// </summary>
	/// <remarks>The handler itself should be configured as a service.</remarks>
	public static IApplicationBuilder UseFacilityHttpHandler<T>(this IApplicationBuilder builder)
		where T : ServiceHttpHandler
	{
		ArgumentNullException.ThrowIfNull(builder);

		return builder.UseMiddleware<FacilityAspNetCoreMiddleware<T>>();
	}

	/// <summary>
	/// Adds a Facility exception handler to the service collection.
	/// </summary>
	/// <remarks>Call <c>app.UseExceptionHandler()</c> to add the exception handler middleware to the pipeline.
	/// Do not include error details in production.</remarks>
	public static IServiceCollection AddFacilityExceptionHandler(this IServiceCollection services, Action<FacilityExceptionHandlerOptions>? configure = null)
	{
		ArgumentNullException.ThrowIfNull(services);

		if (configure is not null)
			services.Configure(configure);

		services.AddExceptionHandler<FacilityExceptionHandler>();
		services.AddProblemDetails();

		return services;
	}

	/// <summary>
	/// Adds a Facility exception handler to the service collection.
	/// </summary>
	/// <remarks>Call <c>app.UseExceptionHandler()</c> to add the exception handler middleware to the pipeline.
	/// Do not include error details in production.</remarks>
	public static IServiceCollection AddFacilityExceptionHandler(this IServiceCollection services, bool includeErrorDetails) =>
		services.AddFacilityExceptionHandler(x => x.IncludeErrorDetails = includeErrorDetails);

	/// <summary>
	/// Adds a Facility service exception handler to the pipeline.
	/// </summary>
	/// <remarks>Do not include error details in production.</remarks>
	[Obsolete("Use AddFacilityExceptionHandler and UseExceptionHandler.")]
	public static IApplicationBuilder UseFacilityExceptionHandler(this IApplicationBuilder builder, Action<FacilityExceptionHandlerOptions>? configure = null)
	{
		ArgumentNullException.ThrowIfNull(builder);

		var options = new FacilityExceptionHandlerOptions();
		configure?.Invoke(options);

		var includeErrorDetails = options.IncludeErrorDetails;
		var contentSerializer = options.ContentSerializer ?? HttpContentSerializer.Create(SystemTextJsonServiceSerializer.Instance);
		var pathPrefixes = options.PathPrefixes;

		return builder.UseExceptionHandler(
			x => x.Run(async context =>
			{
				var effectivePrefixes = pathPrefixes ?? FacilityAspNetCoreUtility.GetPathPrefixesFromRootPath(
					context.RequestServices.GetService<ServiceHttpHandlerSettings>()?.RootPath);
				if (effectivePrefixes is not null)
				{
					var requestPath = context.Request.Path;
					if (!effectivePrefixes.Any(x => requestPath.StartsWithSegments(x, StringComparison.OrdinalIgnoreCase)))
					{
						// not handled; let the default behavior run
						return;
					}
				}

				var cancellationToken = context.RequestAborted;
				var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
				var error = includeErrorDetails && exception != null ? ServiceErrorUtility.CreateInternalErrorForException(exception) : ServiceErrors.CreateInternalError();
				using var httpResponseMessage = FacilityAspNetCoreUtility.CreateHttpResponseMessage(error, contentSerializer);
				await FacilityAspNetCoreUtility.WriteHttpResponseMessageAsync(httpResponseMessage, context.Response, cancellationToken);
			}));
	}

	/// <summary>
	/// Adds a Facility service exception handler to the pipeline.
	/// </summary>
	/// <remarks>Do not include error details in production.</remarks>
	[Obsolete("Use AddFacilityExceptionHandler and UseExceptionHandler.")]
	public static IApplicationBuilder UseFacilityExceptionHandler(this IApplicationBuilder builder, bool includeErrorDetails) =>
		builder.UseFacilityExceptionHandler(x => x.IncludeErrorDetails = includeErrorDetails);
}
