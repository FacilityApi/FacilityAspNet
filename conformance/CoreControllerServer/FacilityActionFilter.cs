using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreControllerServer
{
	internal sealed class FacilityActionFilter : ActionFilterAttribute, IAsyncExceptionFilter
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (context.ActionArguments.ContainsKey(c_httpRequestKey) &&
				context.ActionArguments[c_httpRequestKey].GetType() == typeof(HttpRequestMessage))
			{
				context.ActionArguments[c_httpRequestKey] = FacilityAspNetCoreUtility.CreateHttpRequestMessage(context.HttpContext.Request);
			}
		}

		public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
		{
			if (context.Result is ObjectResult contextResult && contextResult.Value is HttpResponseMessage httpResponseMessage)
				await WriteAndDisposeHttpResponseMessage(httpResponseMessage, context);
			else
				await next();
		}

		public async Task OnExceptionAsync(ExceptionContext context)
		{
			await WriteAndDisposeHttpResponseMessage(FacilityAspNetCoreUtility.CreateHttpResponseMessage(context.Exception), context);
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
}
