using System;
using Facility.Core.Http;
using Microsoft.AspNetCore.Builder;

namespace Facility.AspNetCore.TestServer
{
	/// <summary>
	/// Supports Facility service HTTP handlers.
	/// </summary>
	public static class FacilityHttpHandlerExtensions
	{
		/// <summary>
		/// Adds a Facility service HTTP handler to the pipeline.
		/// </summary>
		/// <remarks>The handler itself should be configured as a service.</remarks>
		public static IApplicationBuilder UseFacilityHttpHandler<T>(this IApplicationBuilder builder)
			where T : ServiceHttpHandler
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			return builder.UseMiddleware<FacilityHandlerMiddleware<T>>();
		}
	}
}
