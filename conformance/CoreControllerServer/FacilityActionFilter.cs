using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Facility.Core;
using Facility.Core.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
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
				var contextRequest = context.HttpContext.Request;
				var encodedUrl = contextRequest.GetEncodedUrl();

				var httpRequestMessage = new HttpRequestMessage(new HttpMethod(contextRequest.Method), encodedUrl)
				{
					Content = new StreamContent(contextRequest.Body),
				};

				foreach (var header in contextRequest.Headers)
				{
					// Every header should be able to fit into one of the two header collections.
					// Try message.Headers first since that accepts more of them.
					if (!httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.AsEnumerable()))
						httpRequestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.AsEnumerable());
				}

				context.ActionArguments[c_httpRequestKey] = httpRequestMessage;
			}
		}

		public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
		{
			if (context.Result is ObjectResult contextResult && contextResult.Value is HttpResponseMessage httpResponseMessage)
				await ProcessResponse(context, httpResponseMessage);
			else
				await next();
		}

		public async Task OnExceptionAsync(ExceptionContext context)
		{
			var error = ServiceErrorUtility.CreateInternalErrorForException(context.Exception);
			var statusCode = HttpServiceErrors.TryGetHttpStatusCode(error.Code) ?? HttpStatusCode.InternalServerError;
			var httpResponseMessage = new HttpResponseMessage(statusCode)
			{
				Content = JsonHttpContentSerializer.Instance.CreateHttpContent(error),
			};
			await ProcessResponse(context, httpResponseMessage);
		}

		private static async Task ProcessResponse(ActionContext context, HttpResponseMessage httpResponseMessage)
		{
			using (httpResponseMessage)
			{
				var contextResponse = context.HttpContext.Response;
				if (!contextResponse.HasStarted)
				{
					contextResponse.StatusCode = (int) httpResponseMessage.StatusCode;

					var responseHeaders = httpResponseMessage.Headers;

					// Ignore the Transfer-Encoding header if it is just "chunked".
					// We let the host decide about whether the response should be chunked or not.
					if (responseHeaders.TransferEncodingChunked == true && responseHeaders.TransferEncoding.Count == 1)
						responseHeaders.TransferEncoding.Clear();

					foreach (var header in responseHeaders)
						contextResponse.Headers.Append(header.Key, header.Value.ToArray());

					if (httpResponseMessage.Content != null)
					{
						var contentHeaders = httpResponseMessage.Content.Headers;

						// Copy the response content headers only after ensuring they are complete.
						// We ask for Content-Length first because HttpContent lazily computes this
						// and only afterwards writes the value into the content headers.
						var unused = contentHeaders.ContentLength;

						foreach (var header in contentHeaders)
							contextResponse.Headers.Append(header.Key, header.Value.ToArray());

						await httpResponseMessage.Content.CopyToAsync(contextResponse.Body).ConfigureAwait(false);
					}
				}
			}
		}

		private const string c_httpRequestKey = "httpRequest";
	}
}
