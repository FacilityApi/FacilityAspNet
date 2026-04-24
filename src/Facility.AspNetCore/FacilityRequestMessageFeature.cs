namespace Facility.AspNetCore;

/// <summary>
/// Exposes the HttpRequestMessage for a Facility API request using HttpContext.Features.
/// </summary>
public sealed class FacilityRequestMessageFeature
{
	/// <summary>
	/// The HttpRequestMessage used by Facility for the API request.
	/// </summary>
	public required HttpRequestMessage RequestMessage { get; init; }
}
