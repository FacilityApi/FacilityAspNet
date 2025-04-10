using System.Diagnostics.CodeAnalysis;
using System.Web.Http;
using Facility.ConformanceApi.Http;

namespace WebApiControllerServer.Controllers;

[SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Generated partial class is public.")]
public partial class ConformanceApiController : ApiController
{
	private ConformanceApiHttpHandler GetServiceHttpHandler() => new(WebApiControllerServerApp.Service);
}
