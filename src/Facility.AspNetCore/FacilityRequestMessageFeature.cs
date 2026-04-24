namespace Facility.AspNetCore;

public sealed class FacilityRequestMessageFeature
{
	public required HttpRequestMessage RequestMessage { get; init; }
}
