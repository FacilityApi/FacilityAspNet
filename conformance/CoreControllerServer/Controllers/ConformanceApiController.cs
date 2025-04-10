using System.Diagnostics.CodeAnalysis;
using Facility.ConformanceApi;
using Facility.ConformanceApi.Http;
using Facility.Core;
using Microsoft.AspNetCore.Mvc;

namespace CoreControllerServer.Controllers;

[SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Generated partial class is public.")]
public partial class ConformanceApiController : Controller
{
	public ConformanceApiController(IConformanceApi api)
	{
		m_api = api;
	}

	// should return 200 with {"id":1} (no name)
	[HttpGet, Route("action/dto")]
	public WidgetDto ServiceDto() => new() { Id = 1 };

	// should return 413 with {"code":"RequestTooLarge","message":"The request is too large."}
	[HttpGet, Route("action/error")]
	public ServiceErrorDto ServiceErrorDto() => ServiceErrors.CreateRequestTooLarge();

	// should return 200 with {"id":1} (no name)
	[HttpGet, Route("action/success")]
	public ServiceResult<WidgetDto> ServiceResultSuccess() => ServiceResult.Success(new WidgetDto { Id = 1 });

	// should return 413 with {"code":"RequestTooLarge","message":"The request is too large."}
	[HttpGet, Route("action/failure")]
	public ServiceResult<WidgetDto> ServiceResultFailure() => ServiceResult.Failure(ServiceErrors.CreateRequestTooLarge());

	private ConformanceApiHttpHandler GetServiceHttpHandler() => new(m_api);

	private readonly IConformanceApi m_api;
}
