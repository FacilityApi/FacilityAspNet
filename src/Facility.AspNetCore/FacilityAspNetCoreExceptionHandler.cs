using System;
using System.Threading.Tasks;
using Facility.Core;
using Microsoft.AspNetCore.Http;

namespace Facility.AspNetCore
{
	/// <summary>
	/// ASP.NET Core middleware for a Facility service exception handler.
	/// </summary>
	public sealed class FacilityAspNetCoreExceptionHandler
	{
		/// <summary>
		/// Creates an instance.
		/// </summary>
		/// <remarks>Do not include error details in production.</remarks>
		public FacilityAspNetCoreExceptionHandler(RequestDelegate next, bool includeErrorDetails = false)
		{
			m_next = next;
			m_includeErrorDetails = includeErrorDetails;
		}

		/// <summary>
		/// Invokes the middleware.
		/// </summary>
		public async Task Invoke(HttpContext httpContext)
		{
			try
			{
				await m_next(httpContext).ConfigureAwait(false);
			}
			catch (Exception exception)
			{
				var error = m_includeErrorDetails ? ServiceErrorUtility.CreateInternalErrorForException(exception) : ServiceErrors.CreateInternalError();
				using var httpResponseMessage = FacilityAspNetCoreUtility.CreateHttpResponseMessage(error);
				await FacilityAspNetCoreUtility.WriteHttpResponseMessageAsync(httpResponseMessage, httpContext.Response);
			}
		}

		private readonly RequestDelegate m_next;
		private readonly bool m_includeErrorDetails;
	}
}
