using Facility.Core.Http;

namespace Facility.AspNetCore;

/// <summary>
/// Options for <see cref="FacilityExceptionHandler" />.
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

	/// <summary>
	/// The path prefixes to handle.
	/// </summary>
	/// <remarks>If empty, no paths are handled.
	/// For example, set to <c>["/v2", "/v3"]</c> to only handle
	/// exceptions for requests whose path starts with <c>/v2</c> or <c>/v3</c>.
	/// If not specified, the `RootPath` from the registered <c>ServiceHttpHandlerSettings</c> is used.
	/// If that is not specified, all paths are handled.</remarks>
	public IReadOnlyList<string>? PathPrefixes { get; set; }
}
