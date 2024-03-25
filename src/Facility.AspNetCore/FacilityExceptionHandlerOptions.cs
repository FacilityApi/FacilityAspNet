using Facility.Core.Http;

namespace Facility.AspNetCore;

/// <summary>
/// Options for <c>FacilityAspNetCoreExtensions.UseFacilityExceptionHandler</c>.
/// </summary>
public sealed class FacilityExceptionHandlerOptions
{
	/// <summary>
	/// True to include error details. Not for production.
	/// </summary>
	public bool IncludeErrorDetails { get; set; }

	/// <summary>
	/// The content serializer to use when writing an error to the response.
	/// </summary>
	public HttpContentSerializer? ContentSerializer { get; set; }
}
