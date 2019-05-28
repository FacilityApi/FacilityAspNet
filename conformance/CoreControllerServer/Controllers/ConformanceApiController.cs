using Facility.ConformanceApi;
using Facility.ConformanceApi.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreControllerServer.Controllers
{
	public partial class ConformanceApiController : Controller
	{
		public ConformanceApiController(IConformanceApi api)
		{
			m_api = api;
		}

		private ConformanceApiHttpHandler GetServiceHttpHandler() => new ConformanceApiHttpHandler(m_api);

		private readonly IConformanceApi m_api;
	}
}
