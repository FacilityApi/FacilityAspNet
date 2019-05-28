using System;
using Facility.Core.Http;
using Microsoft.AspNetCore.Builder;

namespace Facility.AspNetCore
{
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
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			return builder.UseMiddleware<FacilityAspNetCoreMiddleware<T>>();
		}
	}
}
