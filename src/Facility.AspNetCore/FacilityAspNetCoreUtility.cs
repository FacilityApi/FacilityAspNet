using System.Net;
using Facility.Core;
using Facility.Core.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;

namespace Facility.AspNetCore;

/// <summary>
/// Utility methods for using Facility with ASP.NET Core.
/// </summary>
public sealed class FacilityAspNetCoreUtility
{
	/// <summary>
	/// Converts an <c>HttpRequest</c> to an <c>HttpRequestMessage</c>.
	/// </summary>
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

	[Obsolete("Specify contentSerializer.")]
	public static HttpResponseMessage CreateHttpResponseMessage(Exception exception) =>
		CreateHttpResponseMessage(exception, HttpContentSerializer.Create(SystemTextJsonServiceSerializer.Instance));

	[Obsolete("Specify contentSerializer.")]
	public static HttpResponseMessage CreateHttpResponseMessage(ServiceErrorDto error) =>
		CreateHttpResponseMessage(error, HttpContentSerializer.Create(SystemTextJsonServiceSerializer.Instance));

	/// <summary>
	/// Creates an <c>HttpResponseMessage</c> for an exception.
	/// </summary>
	public static HttpResponseMessage CreateHttpResponseMessage(Exception exception, HttpContentSerializer contentSerializer) =>
		CreateHttpResponseMessage(ServiceErrorUtility.CreateInternalErrorForException(exception), contentSerializer);

	/// <summary>
	/// Creates an <c>HttpResponseMessage</c> for an error.
	/// </summary>
	public static HttpResponseMessage CreateHttpResponseMessage(ServiceErrorDto error, HttpContentSerializer contentSerializer)
	{
		var statusCode = HttpServiceErrors.TryGetHttpStatusCode(error.Code) ?? HttpStatusCode.InternalServerError;
		return new HttpResponseMessage(statusCode) { Content = contentSerializer.CreateHttpContent(error) };
	}

	/// <summary>
	/// Writes an <c>HttpResponseMessage</c> to an <c>HttpResponse</c>.
	/// </summary>
	public static Task WriteHttpResponseMessageAsync(HttpResponseMessage httpResponseMessage, HttpResponse contextResponse) =>
		WriteHttpResponseMessageAsync(httpResponseMessage, contextResponse, CancellationToken.None);

	/// <summary>
	/// Writes an <c>HttpResponseMessage</c> to an <c>HttpResponse</c>.
	/// </summary>
	public static async Task WriteHttpResponseMessageAsync(HttpResponseMessage httpResponseMessage, HttpResponse contextResponse, CancellationToken cancellationToken)
	{
		// disable buffering for text/event-stream
		var contentHeaders = httpResponseMessage.Content.Headers;
		if (contentHeaders.ContentType?.MediaType == "text/event-stream")
			contextResponse.HttpContext.Features.Get<IHttpResponseBodyFeature>()?.DisableBuffering();

		contextResponse.StatusCode = (int) httpResponseMessage.StatusCode;

		// Ignore the Transfer-Encoding header if it is just "chunked".
		// We let the host decide about whether the response should be chunked or not.
		var responseHeaders = httpResponseMessage.Headers;
		if (responseHeaders is { TransferEncodingChunked: true, TransferEncoding.Count: 1 })
			responseHeaders.TransferEncoding.Clear();

		foreach (var header in responseHeaders)
			contextResponse.Headers.Append(header.Key, header.Value.ToArray());

		// Copy the response content headers only after ensuring they are complete.
		// We ask for Content-Length first because HttpContent lazily computes this
		// and only afterward writes the value into the content headers.
		_ = contentHeaders.ContentLength;

		foreach (var header in contentHeaders)
			contextResponse.Headers.Append(header.Key, header.Value.ToArray());

		await httpResponseMessage.Content.CopyToAsync(contextResponse.Body, cancellationToken).ConfigureAwait(false);
	}
}
