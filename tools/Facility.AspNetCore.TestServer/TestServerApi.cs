using System.Threading;
using System.Threading.Tasks;
using Facility.Core;
using TestServerApi;

namespace Facility.AspNetCore.TestServer
{
	public sealed class TestServerApi : ITestServerApi
	{
		public async Task<ServiceResult<GetApiInfoResponseDto>> GetApiInfoAsync(GetApiInfoRequestDto request, CancellationToken cancellationToken)
		{
			return ServiceResult.Success(
				new GetApiInfoResponseDto
				{
					Service = "TestServerApi",
					Platform = "AspNetCore",
				});
		}
	}
}
