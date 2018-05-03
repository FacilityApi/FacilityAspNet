using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Facility.Core;
using Facility.Core.Http;
using Microsoft.AspNetCore.Http;

namespace Facility.AspNetCore.TestServer
{
	/// <summary>
	/// ASP.NET Core middleware for a Facility service HTTP handler.
	/// </summary>
	/// <typeparam name="T">The type of the ServiceHttpHandler.</typeparam>
	public sealed class FacilityHandlerMiddleware<T>
		where T : ServiceHttpHandler
	{
		/// <summary>
		/// Creates an instance.
		/// </summary>
		public FacilityHandlerMiddleware(RequestDelegate next, T handler)
		{
			m_next = next;
			m_handler = handler;
		}

		/// <summary>
		/// Invokes the middleware.
		/// </summary>
		/// <remarks>This method translates the ASP.NET Core request into an HttpRequestMessage,
		/// uses the ServiceHttpHandler to handle the message, and then converts the HttpResponseMessage
		/// into an ASP.NET Core response. We borrowed code from Microsoft.AspNetCore.Mvc.WebApiCompatShim
		/// (HttpRequestMessageFeature and HttpResponseMessageOutputFormatter) to make sure we got it right
		/// and to avoid requiring an dependency on ASP.NET Core MVC and WebApiCompatShim.</remarks>
		public async Task Invoke(HttpContext httpContext)
		{
			var httpRequest = httpContext.Request;
			var uriString =
				httpRequest.Scheme + "://" +
				httpRequest.Host +
				httpRequest.PathBase +
				httpRequest.Path +
				httpRequest.QueryString;

			var requestMessage = new HttpRequestMessage(new HttpMethod(httpRequest.Method), uriString);

			// This allows us to pass the message through APIs defined in legacy code and then
			// operate on the HttpContext inside.
			requestMessage.Properties[nameof(HttpContext)] = httpContext;

			requestMessage.Content = new StreamContent(httpRequest.Body);

			foreach (var header in httpRequest.Headers)
			{
				// Every header should be able to fit into one of the two header collections.
				// Try message.Headers first since that accepts more of them.
				if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.AsEnumerable()))
					requestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.AsEnumerable());
			}

			HttpResponseMessage responseMessage;
			try
			{
				responseMessage = await m_handler.TryHandleHttpRequestAsync(requestMessage, httpContext.RequestAborted).ConfigureAwait(false);
			}
			catch (Exception exception)
			{
				var error = ServiceErrorUtility.CreateInternalErrorForException(exception);
				var statusCode = HttpServiceErrors.TryGetHttpStatusCode(error.Code) ?? HttpStatusCode.InternalServerError;
				responseMessage = new HttpResponseMessage(statusCode) { Content = JsonHttpContentSerializer.Instance.CreateHttpContent(error) };
			}

			if (responseMessage != null)
			{
				using (responseMessage)
				{
					var response = httpContext.Response;
					response.StatusCode = (int) responseMessage.StatusCode;

					var responseHeaders = responseMessage.Headers;

					// Ignore the Transfer-Encoding header if it is just "chunked".
					// We let the host decide about whether the response should be chunked or not.
					if (responseHeaders.TransferEncodingChunked == true && responseHeaders.TransferEncoding.Count == 1)
						responseHeaders.TransferEncoding.Clear();

					foreach (var header in responseHeaders)
						response.Headers.Append(header.Key, header.Value.ToArray());

					if (responseMessage.Content != null)
					{
						var contentHeaders = responseMessage.Content.Headers;

						// Copy the response content headers only after ensuring they are complete.
						// We ask for Content-Length first because HttpContent lazily computes this
						// and only afterwards writes the value into the content headers.
						var unused = contentHeaders.ContentLength;

						foreach (var header in contentHeaders)
							response.Headers.Append(header.Key, header.Value.ToArray());

						await responseMessage.Content.CopyToAsync(response.Body).ConfigureAwait(false);
					}
				}
			}
			else
			{
				await m_next(httpContext).ConfigureAwait(false);
			}
		}

		readonly RequestDelegate m_next;
		readonly T m_handler;
	}
}
