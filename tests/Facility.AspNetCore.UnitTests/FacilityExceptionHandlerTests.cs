using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Facility.AspNetCore.UnitTests;

internal sealed class FacilityExceptionHandlerTests
{
	[Test]
	public async Task HandledExceptionIsLogged()
	{
		var logger = new TestLogger<FacilityExceptionHandler>();
		var options = Options.Create(new FacilityExceptionHandlerOptions());
		var handler = new FacilityExceptionHandler(options, logger);
		var httpContext = CreateHttpContext("/api/widgets");
		var exception = new InvalidOperationException("boom");

		var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

		handled.Should().BeTrue();
		logger.Entries.Should().ContainSingle();
		logger.Entries[0].LogLevel.Should().Be(LogLevel.Error);
		logger.Entries[0].Exception.Should().BeSameAs(exception);
	}

	[Test]
	public async Task UnhandledExceptionIsNotLogged()
	{
		var logger = new TestLogger<FacilityExceptionHandler>();
		var options = Options.Create(new FacilityExceptionHandlerOptions { PathPrefixes = ["/api"] });
		var handler = new FacilityExceptionHandler(options, logger);
		var httpContext = CreateHttpContext("/other/widgets");

		var handled = await handler.TryHandleAsync(httpContext, new InvalidOperationException("boom"), CancellationToken.None);

		handled.Should().BeFalse();
		logger.Entries.Should().BeEmpty();
	}

	private static DefaultHttpContext CreateHttpContext(string path)
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Request.Path = path;
		httpContext.RequestServices = new ServiceCollection().BuildServiceProvider();
		httpContext.Response.Body = new MemoryStream();
		return httpContext;
	}

	internal sealed record LogEntry(LogLevel LogLevel, Exception? Exception, string Message);

	private sealed class TestLogger<T> : ILogger<T>
	{
		public IDisposable? BeginScope<TState>(TState state)
			where TState : notnull =>
			null;

		public bool IsEnabled(LogLevel logLevel) => true;

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
			Func<TState, Exception?, string> formatter)
		{
			Entries.Add(new LogEntry(logLevel, exception, formatter(state, exception)));
		}

		public List<LogEntry> Entries { get; } = [];
	}
}
