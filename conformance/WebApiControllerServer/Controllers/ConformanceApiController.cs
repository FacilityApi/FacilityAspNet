using System.Web.Http;
using Facility.ConformanceApi.Http;

namespace WebApiControllerServer.Controllers
{
	public partial class ConformanceApiController : ApiController
	{
		private ConformanceApiHttpHandler GetServiceHttpHandler() => new ConformanceApiHttpHandler(WebApiControllerServerApp.Service);
	}
}
