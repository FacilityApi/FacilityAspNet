using Facility.Core;
using Facility.Core.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;

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
	/// Adds a Facility service exception handler to the pipeline.
	/// </summary>
	/// <remarks>Do not include error details in production.</remarks>
	public static IApplicationBuilder UseFacilityExceptionHandler(this IApplicationBuilder builder, Action<FacilityExceptionHandlerOptions>? configure = null)
	{
		ArgumentNullException.ThrowIfNull(builder);

		var options = new FacilityExceptionHandlerOptions();
		configure?.Invoke(options);

		var includeErrorDetails = options.IncludeErrorDetails;
		var contentSerializer = options.ContentSerializer ?? HttpContentSerializer.Create(SystemTextJsonServiceSerializer.Instance);

		return builder.UseExceptionHandler(
			x => x.Run(async context =>
			{
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
	public static IApplicationBuilder UseFacilityExceptionHandler(this IApplicationBuilder builder, bool includeErrorDetails) =>
		builder.UseFacilityExceptionHandler(x => x.IncludeErrorDetails = includeErrorDetails);
}
