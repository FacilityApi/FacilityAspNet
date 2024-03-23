using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Facility.AspNetCore;

public sealed class FacilityActionFilter : ActionFilterAttribute
{
	public override void OnActionExecuting(ActionExecutingContext context)
	{
		if (context.ActionArguments.TryGetValue(c_httpRequestKey, out var request) && request?.GetType() == typeof(HttpRequestMessage))
			context.ActionArguments[c_httpRequestKey] = FacilityAspNetCoreUtility.CreateHttpRequestMessage(context.HttpContext.Request);
	}

	public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
	{
		if (context.Result is ObjectResult { Value: HttpResponseMessage httpResponseMessage })
			await WriteAndDisposeHttpResponseMessage(httpResponseMessage, context);
		else
			await next();
	}

	private static async Task WriteAndDisposeHttpResponseMessage(HttpResponseMessage httpResponseMessage, ActionContext context)
	{
		using (httpResponseMessage)
		{
			var contextResponse = context.HttpContext.Response;
			if (!contextResponse.HasStarted)
				await FacilityAspNetCoreUtility.WriteHttpResponseMessageAsync(httpResponseMessage, contextResponse);
		}
	}

	private const string c_httpRequestKey = "httpRequest";
}
