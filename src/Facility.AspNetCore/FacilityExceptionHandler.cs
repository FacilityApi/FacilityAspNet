using Facility.Core;
using Facility.Core.Http;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Facility.AspNetCore;

/// <summary>
/// An ASP.NET Core exception handler that returns Facility service errors.
/// </summary>
public sealed class FacilityExceptionHandler : IExceptionHandler
{
	/// <summary>
	/// Creates an instance.
	/// </summary>
	public FacilityExceptionHandler(IOptions<FacilityExceptionHandlerOptions> options)
	{
		m_includeErrorDetails = options.Value.IncludeErrorDetails;
		m_contentSerializer = options.Value.ContentSerializer ?? HttpContentSerializer.Create(SystemTextJsonServiceSerializer.Instance);
	}

	/// <inheritdoc />
	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		var error = m_includeErrorDetails ? ServiceErrorUtility.CreateInternalErrorForException(exception) : ServiceErrors.CreateInternalError();

		using var httpResponseMessage = FacilityAspNetCoreUtility.CreateHttpResponseMessage(error, m_contentSerializer);
		await FacilityAspNetCoreUtility.WriteHttpResponseMessageAsync(httpResponseMessage, httpContext.Response, cancellationToken);

		return true;
	}

	private readonly bool m_includeErrorDetails;
	private readonly HttpContentSerializer m_contentSerializer;
}
