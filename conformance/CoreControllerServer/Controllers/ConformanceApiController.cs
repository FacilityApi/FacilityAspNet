using Facility.ConformanceApi;
using Facility.ConformanceApi.Http;
using Facility.Core;
using Microsoft.AspNetCore.Mvc;

namespace CoreControllerServer.Controllers;

public partial class ConformanceApiController : Controller
{
	public ConformanceApiController(IConformanceApi api)
	{
		m_api = api;
	}

	// should return 200 with {"id":1} (no name)
	[HttpGet, Route("ServiceDto")]
	public WidgetDto ServiceDto() => new() { Id = 1 };

	// should return 200 with {"id":1} (no name)
	[HttpGet, Route("ServiceResultSuccess")]
	public ServiceResult<WidgetDto> ServiceResultSuccess() => ServiceResult.Success(new WidgetDto { Id = 1 });

	// should return 413 with {"code":"RequestTooLarge","message":"The request is too large."}
	[HttpGet, Route("ServiceResultFailure")]
	public ServiceResult<WidgetDto> ServiceResultFailure() => ServiceResult.Failure(ServiceErrors.CreateRequestTooLarge());

	// should return 413 with {"code":"RequestTooLarge","message":"The request is too large."}
	[HttpGet, Route("ServiceErrorDto")]
	public ServiceErrorDto ServiceErrorDto() => ServiceErrors.CreateRequestTooLarge();

	private ConformanceApiHttpHandler GetServiceHttpHandler() => new(m_api);

	private readonly IConformanceApi m_api;
}
