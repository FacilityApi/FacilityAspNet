using System;
using System.Net.Http;
using System.Threading.Tasks;
using Facility.Core.Http;
using Microsoft.AspNetCore.Http;

namespace Facility.AspNetCore
{
	/// <summary>
	/// ASP.NET Core middleware for a Facility service HTTP handler.
	/// </summary>
	/// <typeparam name="T">The type of the ServiceHttpHandler.</typeparam>
	public sealed class FacilityAspNetCoreMiddleware<T>
		where T : ServiceHttpHandler
	{
		/// <summary>
		/// Creates an instance.
		/// </summary>
		public FacilityAspNetCoreMiddleware(RequestDelegate next, T handler)
		{
			m_next = next;
			m_handler = handler;
		}

		/// <summary>
		/// Invokes the middleware.
		/// </summary>
		public async Task Invoke(HttpContext httpContext)
		{
			var httpRequestMessage = FacilityAspNetCoreUtility.CreateHttpRequestMessage(httpContext.Request);

			HttpResponseMessage? httpResponseMessage;
			try
			{
				httpResponseMessage = await m_handler.TryHandleHttpRequestAsync(httpRequestMessage, httpContext.RequestAborted).ConfigureAwait(false);
			}
			catch (Exception exception)
			{
				httpResponseMessage = FacilityAspNetCoreUtility.CreateHttpResponseMessage(exception);
			}

			if (httpResponseMessage != null)
			{
				using (httpResponseMessage)
					await FacilityAspNetCoreUtility.WriteHttpResponseMessageAsync(httpResponseMessage, httpContext.Response);
			}
			else
			{
				await m_next(httpContext).ConfigureAwait(false);
			}
		}

		private readonly RequestDelegate m_next;
		private readonly T m_handler;
	}
}
