using Facility.ConformanceApi;
using Facility.ConformanceApi.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApiShimServer.Controllers
{
	public partial class ConformanceApiController : ControllerBase
	{
		public ConformanceApiController(IConformanceApi api)
		{
			m_api = api;
		}

		private ConformanceApiHttpHandler GetServiceHttpHandler() => new(m_api);

		private readonly IConformanceApi m_api;
	}
}
