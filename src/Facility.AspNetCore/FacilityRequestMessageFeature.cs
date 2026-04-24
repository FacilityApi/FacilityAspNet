namespace Facility.AspNetCore;

/// <summary>
/// Exposes the HttpRequestMessage for a facility API request on the http context.
/// </summary>
public sealed class FacilityRequestMessageFeature
{
	/// <summary>
	/// The HttpRequestMessage used by facility for the API request.
	/// </summary>
	public required HttpRequestMessage RequestMessage { get; init; }
}
