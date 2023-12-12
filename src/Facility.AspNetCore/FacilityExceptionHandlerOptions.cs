using Facility.Core.Http;

namespace Facility.AspNetCore;

public sealed class FacilityExceptionHandlerOptions
{
	public bool IncludeErrorDetails { get; set; }

	public HttpContentSerializer? ContentSerializer { get; set; }
}
