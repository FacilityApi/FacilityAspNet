using System.Net;
using Facility.Core;
using Facility.Core.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Facility.AspNetCore;

public sealed class FacilityActionFilter : ActionFilterAttribute
{
	public override void OnActionExecuting(ActionExecutingContext context)
	{
		// replace empty httpRequest argument with the actual request
		if (context.ActionArguments.TryGetValue(c_httpRequestKey, out var request) && request?.GetType() == typeof(HttpRequestMessage))
			context.ActionArguments[c_httpRequestKey] = FacilityAspNetCoreUtility.CreateHttpRequestMessage(context.HttpContext.Request);
	}

	public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
	{
		// special response handling for Facility types
		if (context.Result is ObjectResult { Value: { } resultObject } && TryCreateHttpResponseMessage(resultObject) is { } httpResponseMessage)
			await WriteAndDisposeHttpResponseMessage(httpResponseMessage, context);
		else
			await next();
	}

	private static HttpResponseMessage? TryCreateHttpResponseMessage(object resultObject) =>
		resultObject switch
		{
			HttpResponseMessage message => message,
			ServiceErrorDto error => FacilityAspNetCoreUtility.CreateHttpResponseMessage(error, s_httpContentSerializer),
			ServiceResult { IsFailure: true } failure => FacilityAspNetCoreUtility.CreateHttpResponseMessage(failure.Error ?? new(), s_httpContentSerializer),
			ServiceResult success => ServiceResultUtility.TryGetValue(success, out var value) && value is not null
				? new HttpResponseMessage(HttpStatusCode.OK) { Content = s_httpContentSerializer.CreateHttpContent(value) }
				: new HttpResponseMessage(HttpStatusCode.OK),
			ServiceDto dto => new HttpResponseMessage(HttpStatusCode.OK) { Content = s_httpContentSerializer.CreateHttpContent(dto) },
			_ => null,
		};

	private static async Task WriteAndDisposeHttpResponseMessage(HttpResponseMessage httpResponseMessage, ActionContext context)
	{
		using (httpResponseMessage)
		{
			var contextResponse = context.HttpContext.Response;
			if (!contextResponse.HasStarted)
				await FacilityAspNetCoreUtility.WriteHttpResponseMessageAsync(httpResponseMessage, contextResponse);
		}
	}

	private static readonly HttpContentSerializer s_httpContentSerializer = HttpContentSerializer.Create(SystemTextJsonServiceSerializer.Instance);

	private const string c_httpRequestKey = "httpRequest";
}
