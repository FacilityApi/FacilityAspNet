#if NET7_0_OR_GREATER
using System.Net;
using Facility.Core;
using Facility.Core.Http;
using Microsoft.AspNetCore.Http;

namespace Facility.AspNetCore;

/// <summary>
/// Supports Facility types returned from API routes (<c>ServiceResult</c> and <c>ServiceError</c>).
/// </summary>
public sealed class FacilityEndpointFilter : IEndpointFilter
{
	/// <inheritdoc />
	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		var result = await next(context);

		if (result is ServiceResult serviceResult)
		{
			if (serviceResult.IsSuccess)
				return ServiceResultUtility.TryGetValue(serviceResult, out var value) && value is not null ? Results.Ok(value) : Results.Ok();

			result = serviceResult.Error;
		}

		if (result is ServiceErrorDto serviceError)
			return Results.Json(serviceError, statusCode: (int) (HttpServiceErrors.TryGetHttpStatusCode(serviceError.Code) ?? HttpStatusCode.InternalServerError));

		return result;
	}
}
#endif
