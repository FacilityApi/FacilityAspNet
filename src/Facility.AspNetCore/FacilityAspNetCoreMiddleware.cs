using Facility.Core.Http;
using Microsoft.AspNetCore.Http;

namespace Facility.AspNetCore;

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
	public FacilityAspNetCoreMiddleware(RequestDelegate next)
	{
		m_next = next;
	}

	/// <summary>
	/// Invokes the middleware.
	/// </summary>
	public async Task Invoke(HttpContext httpContext, T handler)
	{
		var cancellationToken = httpContext.RequestAborted;
		var httpRequestMessage = FacilityAspNetCoreUtility.CreateHttpRequestMessage(httpContext.Request);
		var httpResponseMessage = await handler.TryHandleHttpRequestAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);
		if (httpResponseMessage != null)
		{
			using (httpResponseMessage)
				await FacilityAspNetCoreUtility.WriteHttpResponseMessageAsync(httpResponseMessage, httpContext.Response, cancellationToken);
		}
		else
		{
			await m_next(httpContext).ConfigureAwait(false);
		}
	}

	private readonly RequestDelegate m_next;
}
