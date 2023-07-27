using System.Net;
using Facility.Core;
using Facility.Core.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Facility.AspNetCore
{
	public sealed class FacilityAspNetCoreUtility
	{
		public static HttpRequestMessage CreateHttpRequestMessage(HttpRequest httpRequest)
		{
			var encodedUrl = httpRequest.GetEncodedUrl();

			var httpRequestMessage = new HttpRequestMessage(new HttpMethod(httpRequest.Method), encodedUrl)
			{
				Content = new StreamContent(httpRequest.Body),
			};

			foreach (var header in httpRequest.Headers)
			{
				// Every header should be able to fit into one of the two header collections.
				// Try message.Headers first since that accepts more of them.
				if (!httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.AsEnumerable()))
					httpRequestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.AsEnumerable());
			}

			return httpRequestMessage;
		}

		public static HttpResponseMessage CreateHttpResponseMessage(Exception exception) =>
			CreateHttpResponseMessage(ServiceErrorUtility.CreateInternalErrorForException(exception));

		public static HttpResponseMessage CreateHttpResponseMessage(ServiceErrorDto error)
		{
			var statusCode = HttpServiceErrors.TryGetHttpStatusCode(error.Code) ?? HttpStatusCode.InternalServerError;
			return new HttpResponseMessage(statusCode)
			{
				Content = HttpContentSerializer.Create(SystemTextJsonServiceSerializer.Instance).CreateHttpContent(error),
			};
		}

		public static async Task WriteHttpResponseMessageAsync(HttpResponseMessage httpResponseMessage, HttpResponse contextResponse)
		{
			contextResponse.StatusCode = (int) httpResponseMessage.StatusCode;

			var responseHeaders = httpResponseMessage.Headers;

			// Ignore the Transfer-Encoding header if it is just "chunked".
			// We let the host decide about whether the response should be chunked or not.
			if (responseHeaders.TransferEncodingChunked == true && responseHeaders.TransferEncoding.Count == 1)
				responseHeaders.TransferEncoding.Clear();

			foreach (var header in responseHeaders)
				contextResponse.Headers.Append(header.Key, header.Value.ToArray());

			var contentHeaders = httpResponseMessage.Content.Headers;

			// Copy the response content headers only after ensuring they are complete.
			// We ask for Content-Length first because HttpContent lazily computes this
			// and only afterwards writes the value into the content headers.
			_ = contentHeaders.ContentLength;

			foreach (var header in contentHeaders)
				contextResponse.Headers.Append(header.Key, header.Value.ToArray());

			await httpResponseMessage.Content.CopyToAsync(contextResponse.Body).ConfigureAwait(false);
		}
	}
}
