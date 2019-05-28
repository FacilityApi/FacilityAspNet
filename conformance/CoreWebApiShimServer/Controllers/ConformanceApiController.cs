using System.Web.Http;
using Facility.ConformanceApi;
using Facility.ConformanceApi.Http;

namespace CoreWebApiShimServer.Controllers
{
	public partial class ConformanceApiController : ApiController
	{
		public ConformanceApiController(IConformanceApi api)
		{
			m_api = api;
		}

		private ConformanceApiHttpHandler GetServiceHttpHandler() => new ConformanceApiHttpHandler(m_api);

		private readonly IConformanceApi m_api;
	}
}
