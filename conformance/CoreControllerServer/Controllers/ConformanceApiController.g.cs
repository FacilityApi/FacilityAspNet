// DO NOT EDIT: generated by fsdgenaspnet
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Facility.ConformanceApi;
using Facility.Core;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable 1591 // missing XML comment
#pragma warning disable ASP0018 // Unused route parameter

namespace CoreControllerServer.Controllers
{
	[System.CodeDom.Compiler.GeneratedCode("fsdgenaspnet", "")]
	public partial class ConformanceApiController
	{
		[HttpGet, Route("")]
		public Task<HttpResponseMessage> GetApiInfo(HttpRequestMessage httpRequest, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetServiceHttpHandler().TryHandleGetApiInfoAsync(httpRequest, cancellationToken);
		}

		[HttpGet, Route("widgets")]
		public Task<HttpResponseMessage> GetWidgets(HttpRequestMessage httpRequest, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetServiceHttpHandler().TryHandleGetWidgetsAsync(httpRequest, cancellationToken);
		}

		[HttpPost, Route("widgets")]
		public Task<HttpResponseMessage> CreateWidget(HttpRequestMessage httpRequest, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetServiceHttpHandler().TryHandleCreateWidgetAsync(httpRequest, cancellationToken);
		}

		[HttpGet, Route("widgets/{id}")]
		public Task<HttpResponseMessage> GetWidget(HttpRequestMessage httpRequest, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetServiceHttpHandler().TryHandleGetWidgetAsync(httpRequest, cancellationToken);
		}

		[HttpDelete, Route("widgets/{id}")]
		public Task<HttpResponseMessage> DeleteWidget(HttpRequestMessage httpRequest, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetServiceHttpHandler().TryHandleDeleteWidgetAsync(httpRequest, cancellationToken);
		}

		[HttpPost, Route("widgets/get")]
		public Task<HttpResponseMessage> GetWidgetBatch(HttpRequestMessage httpRequest, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetServiceHttpHandler().TryHandleGetWidgetBatchAsync(httpRequest, cancellationToken);
		}

		[HttpPost, Route("mirrorFields")]
		public Task<HttpResponseMessage> MirrorFields(HttpRequestMessage httpRequest, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetServiceHttpHandler().TryHandleMirrorFieldsAsync(httpRequest, cancellationToken);
		}

		[HttpGet, Route("checkQuery")]
		public Task<HttpResponseMessage> CheckQuery(HttpRequestMessage httpRequest, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetServiceHttpHandler().TryHandleCheckQueryAsync(httpRequest, cancellationToken);
		}

		[HttpGet, Route("checkPath/{string}/{boolean}/{double}/{int32}/{int64}/{decimal}/{enum}/{datetime}")]
		public Task<HttpResponseMessage> CheckPath(HttpRequestMessage httpRequest, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetServiceHttpHandler().TryHandleCheckPathAsync(httpRequest, cancellationToken);
		}

		[HttpGet, Route("mirrorHeaders")]
		public Task<HttpResponseMessage> MirrorHeaders(HttpRequestMessage httpRequest, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetServiceHttpHandler().TryHandleMirrorHeadersAsync(httpRequest, cancellationToken);
		}

		[HttpPost, Route("mixed/{path}")]
		public Task<HttpResponseMessage> Mixed(HttpRequestMessage httpRequest, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetServiceHttpHandler().TryHandleMixedAsync(httpRequest, cancellationToken);
		}

		[HttpPost, Route("required")]
		public Task<HttpResponseMessage> Required(HttpRequestMessage httpRequest, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetServiceHttpHandler().TryHandleRequiredAsync(httpRequest, cancellationToken);
		}

		[HttpPost, Route("mirrorBytes")]
		public Task<HttpResponseMessage> MirrorBytes(HttpRequestMessage httpRequest, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetServiceHttpHandler().TryHandleMirrorBytesAsync(httpRequest, cancellationToken);
		}

		[HttpPost, Route("mirrorText")]
		public Task<HttpResponseMessage> MirrorText(HttpRequestMessage httpRequest, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetServiceHttpHandler().TryHandleMirrorTextAsync(httpRequest, cancellationToken);
		}

		[HttpPost, Route("bodyTypes")]
		public Task<HttpResponseMessage> BodyTypes(HttpRequestMessage httpRequest, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetServiceHttpHandler().TryHandleBodyTypesAsync(httpRequest, cancellationToken);
		}
	}
}
